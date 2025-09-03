using System.Diagnostics.CodeAnalysis;
using Content.Shared._Coyote.RolePlayIncentiveShared;
using Content.Shared.Alert;
using Content.Shared.Humanoid;
using Content.Shared.IdentityManagement;
using Content.Shared.Movement.Systems;
using Content.Shared.StatusIcon;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared._Coyote.Needs;

/// <summary>
/// A datum that holds information about a specific need
/// Also holds the logic for decaying the need over time
/// And some other stuff
/// Starts life blank, and needs to be filled out by the NeedsComponent
/// And it fills itself out using the NeedPrototype~
/// </summary>
public sealed class NeedDatum()
{
    /// <summary>
    /// The type of need this datum represents
    /// </summary>
    public NeedType NeedType = NeedType.Hunger;

    /// <summary>
    /// The prototype ID of the need this datum represents
    /// </summary>
    public ProtoId<NeedPrototype> PrototypeId = default!;

    /// <summary>
    /// The name of the need
    /// </summary>
    public string NeedName = "Busty Vixens";

    /// <summary>
    /// Color associated with this need, for text and icons
    /// </summary>
    public Color NeedColor = Color.White;

    /// <summary>
    /// The current value of the need
    /// </summary>
    public float CurrentValue = 100.0f;

    /// <summary>
    /// The maximum value of the need
    /// </summary>
    public float MaxValue = 100.0f;

    /// <summary>
    /// The minimum value of the need
    /// </summary>
    public float MinValue = 0.0f;

    /// <summary>
    /// The rate at which the need decays over time (per second)
    /// </summary>
    public float DecayRate = 0.0f;

    /// <summary>
    /// The thresholds for this need
    /// </summary>
    public Dictionary<NeedThreshold, float> Thresholds = new();

    /// <summary>
    /// The alerts for this need
    /// </summary>
    public Dictionary<NeedThreshold, ProtoId<AlertPrototype>?> Alerts = new();

    /// <summary>
    /// The hud icon... things for this need
    /// </summary>
    public Dictionary<NeedThreshold, string> StatusIcons = new();

    /// <summary>
    /// The slowdown modifiers for this need
    /// </summary>
    public Dictionary<NeedThreshold, float> SlowdownModifiers = new();

    /// <summary>
    /// The RPI modifiers for this need
    /// </summary>
    public Dictionary<NeedThreshold, float> RpiModifiers = new();

    /// <summary>
    /// The current threshold that the need is in
    /// </summary>
    public NeedThreshold CurrentThreshold = NeedThreshold.Satisfied;

    /// <summary>
    /// The Alert Category associated with this need, if any.
    /// </summary>
    public ProtoId<AlertCategoryPrototype> AlertCategory;

    /// <summary>
    /// Rate it updates in seconds.
    /// </summary>
    public TimeSpan UpdateRateSeconds = TimeSpan.FromSeconds(1f);

    /// <summary>
    /// Next update time.
    /// </summary>
    public TimeSpan NextUpdateTime = TimeSpan.Zero;

    // Localization stuff
    public string ExamineTextKey = "thirst";

    /// <summary>
    /// Constructor for the NeedDatum
    /// Takes in a NeedPrototype and fills out the datum
    /// </summary>

    #region Constructor

    public NeedDatum(NeedPrototype proto) : this()
    {
        if (proto is not { ID: not null or not "" })
            throw new ArgumentException("NeedPrototype must have a valid ID");
        PrototypeId = proto.ID ?? throw new ArgumentException("NeedPrototype must have a valid ID");
        NeedName = proto.NeedName; // dont call me needy
        if (Enum.TryParse<NeedType>(proto.NeedKind, out var needType))
        {
            NeedType = needType;
        }
        else
        {
            throw new ArgumentException($"Invalid NeedType in NeedPrototype: {proto.NeedKind}");
        }

        var hazCoolor = Color.TryFromName(proto.NeedColor, out var doColor);
        NeedColor = hazCoolor ? doColor : Color.White;
        MaxValue = proto.MaxValue;
        MinValue = proto.MinValue;
        AlertCategory = proto.AlertCategory;
        UpdateRateSeconds = TimeSpan.FromSeconds(proto.UpdateRateSeconds);
        CalcualteDecayRate(proto);
        CalcualteInitialValue(proto);
        FillOutThresholds(proto);
        FillOutAlerts(proto);
        FillOutStatusIcons(proto);
        FillOutSlowdownModifiers(proto);
        FillOutRpiModifiers(proto);
        UpdateCurrentThreshold();
    }

    #endregion

    #region Setup Helpers

    /// <summary>
    /// Takes in the time in minutes it should take to go from max to min, and calculates the decay rate
    /// In units per second
    /// </summary>
    private void CalcualteDecayRate(NeedPrototype proto)
    {
        var proMinutes = proto.MinutesFromMaxToMin * proto.TimeScalar;
        if (proMinutes <= 0)
        {
            DecayRate = 1.0f; // Default decay rate if invalid value is provided
            throw new ArgumentException("MinutesFromMaxToMin must be greater than 0");
        }

        DecayRate = MaxValue / (float)(proMinutes * 60.0);
    }

    /// <summary>
    /// Takes in the starting time in minutes and calculates the initial value of the need
    /// Basically, we start at max, and decay for the starting time
    /// </summary>
    private void CalcualteInitialValue(NeedPrototype proto)
    {
        if (proto.StartingMinutesWorthOfDecay < 0)
        {
            CurrentValue = MaxValue;
            return;
        }

        CurrentValue = MaxValue - (DecayRate * (float)(proto.StartingMinutesWorthOfDecay * 60.0 * proto.TimeScalar));
    }

    /// <summary>
    /// Creates the thresholds dictionary from the prototype
    /// </summary>
    private void FillOutThresholds(NeedPrototype proto)
    {
        Thresholds[NeedThreshold.ExtraSatisfied] = proto.ExtraSatisfiedMinutesFromFull;
        Thresholds[NeedThreshold.Satisfied] = proto.SatisfiedMinutesFromFull;
        Thresholds[NeedThreshold.Low] = proto.LowMinutesFromFull;
        Thresholds[NeedThreshold.Critical] = float.MaxValue; // ensure its the lowest threshold
        // Convert minutes to actual values
        foreach (var key in Thresholds.Keys)
        {
            Thresholds[key] = MaxValue - (DecayRate * (Thresholds[key] * 60.0f) * proto.TimeScalar);
            Thresholds[key] = Math.Clamp(
                Thresholds[key],
                MinValue,
                MaxValue);
        }
    }

    /// <summary>
    /// Adds the alerts from the prototype to the datum, filling in nulls where necessary
    /// </summary>
    private void FillOutAlerts(NeedPrototype proto)
    {
        Alerts[NeedThreshold.ExtraSatisfied] = proto.ExtraSatisfiedAlert;
        Alerts[NeedThreshold.Satisfied] = proto.SatisfiedAlert;
        Alerts[NeedThreshold.Low] = proto.LowAlert;
        Alerts[NeedThreshold.Critical] = proto.CriticalAlert;
    }

    /// <summary>
    /// Adds the status icons from the prototype to the datum, filling in empty strings where necessary
    /// </summary>
    private void FillOutStatusIcons(NeedPrototype proto)
    {
        StatusIcons[NeedThreshold.ExtraSatisfied] = proto.ExtraSatisfiedIcon ?? string.Empty;
        StatusIcons[NeedThreshold.Satisfied] = proto.SatisfiedIcon ?? string.Empty;
        StatusIcons[NeedThreshold.Low] = proto.LowIcon ?? string.Empty;
        StatusIcons[NeedThreshold.Critical] = proto.CriticalIcon ?? string.Empty;
    }

    /// <summary>
    /// Adds the slowdown modifiers from the prototype to the datum, filling in 1.0s where necessary
    /// </summary>
    private void FillOutSlowdownModifiers(NeedPrototype proto)
    {
        SlowdownModifiers[NeedThreshold.ExtraSatisfied] = proto.ExtraSatisfiedSlowdown;
        SlowdownModifiers[NeedThreshold.Satisfied] = proto.SatisfiedSlowdown;
        SlowdownModifiers[NeedThreshold.Low] = proto.LowSlowdown;
        SlowdownModifiers[NeedThreshold.Critical] = proto.CriticalSlowdown;
        // clamp to 'reasonable' values
        foreach (var key in SlowdownModifiers.Keys)
        {
            SlowdownModifiers[key] = Math.Clamp(
                SlowdownModifiers[key],
                0.05f,
                10.0f);
        }
    }

    /// <summary>
    /// Adds the RPI modifiers from the prototype to the datum, filling in 1.0s where necessary
    /// </summary>
    private void FillOutRpiModifiers(NeedPrototype proto)
    {
        RpiModifiers[NeedThreshold.ExtraSatisfied] = proto.ExtraSatisfiedRoleplayIncentive;
        RpiModifiers[NeedThreshold.Satisfied] = proto.SatisfiedRoleplayIncentive;
        RpiModifiers[NeedThreshold.Low] = proto.LowRoleplayIncentive;
        RpiModifiers[NeedThreshold.Critical] = proto.CriticalRoleplayIncentive;
        // clamp to 'reasonable' values
        foreach (var key in RpiModifiers.Keys)
        {
            RpiModifiers[key] = Math.Clamp(
                RpiModifiers[key],
                0.05f,
                10.0f);
        }
    }

    #endregion

    public ProtoId<AlertPrototype> GetCurrentAlert()
    {
        if (!Alerts.TryGetValue(CurrentThreshold, out var alert))
        {
            return default;
        }

        return alert ?? default;
    }

    /// <summary>
    /// Decays the need over time
    /// </summary>
    /// <param name="deltaTime">The time since the last update (in seconds)</param>
    public void Decay(float deltaTime)
    {
        CurrentValue -= DecayRate * deltaTime;
        CurrentValue = Math.Clamp(
            CurrentValue,
            MinValue,
            MaxValue);
    }

    /// <summary>
    /// Modifies the current value of the need by a specified amount
    /// </summary>
    public void ModifyCurrentValue(float amount)
    {
        CurrentValue += amount;
        CurrentValue = Math.Clamp(
            CurrentValue,
            MinValue,
            MaxValue);
    }

    /// <summary>
    /// Sets the current value of the need to a specified amount
    /// </summary>
    public void SetCurrentValue(float amount)
    {
        CurrentValue = Math.Clamp(
            amount,
            MinValue,
            MaxValue);
    }

    /// <summary>
    /// Gets the current threshold of the need based on its current value
    /// Its the threshold with the highest minimum value that is less than or equal to the current value
    /// </summary>
    public NeedThresholdUpdateResult UpdateCurrentThreshold()
    {
        var oldThreshold = CurrentThreshold;
        var current = GetThresholdForValue(CurrentValue);
        CurrentThreshold = current;
        return new NeedThresholdUpdateResult(oldThreshold, current);
    }

    public NeedThreshold GetThresholdForValue(float value)
    {
        var outThresh = NeedThreshold.Critical; // Start at the lowest threshold
        var highestMinValue = float.MinValue;
        foreach (var (threshold, minValue) in Thresholds)
        {
            if (value >= minValue)
            {
                if (minValue > highestMinValue)
                {
                    highestMinValue = minValue;
                    outThresh = threshold;
                }
            }
        }

        return outThresh;
    }

    public float GetValueForThreshold(NeedThreshold threshold)
    {
        Thresholds.TryGetValue(threshold, out var value);
        return value;
    }

    public bool IsBelowThreshold(NeedThreshold threshold)
    {
        var threshValue = Thresholds[threshold];
        return CurrentValue < threshValue;
    }

    /// <summary>
    /// Modifies the RPI event multiplier based on the current threshold
    /// </summary>
    public void ModifyRpiEvent(ref GetRoleplayIncentiveModifier ev)
    {
        if (RpiModifiers.TryGetValue(CurrentThreshold, out var modifier))
        {
            ev.Modify(modifier, 0.0f);
        }
    }

    /// <summary>
    /// Modifies the movement speed based on the current threshold
    /// </summary>
    public void ApplyMovementSpeedModifier(ref RefreshMovementSpeedModifiersEvent args)
    {
        if (SlowdownModifiers.TryGetValue(CurrentThreshold, out var modifier))
        {
            args.ModifySpeed(modifier, modifier);
        }
    }

    /// <summary>
    /// Gets the StatusIcon for the current threshold, if any
    /// </summary>
    public bool GetCurrentStatusIcon([NotNullWhen(true)] out string? icon)
    {
        if (StatusIcons.TryGetValue(CurrentThreshold, out var iconId)
            && !string.IsNullOrEmpty(iconId))
        {
            icon = iconId;
            return true;
        }

        icon = null;
        return false;
    }

}

/// <summary>
/// An event raised when something ELSE wants to mess with the examine text
/// </summary>
public sealed class NeedExamineInfoEvent(NeedDatum need, EntityUid examinee, bool isSelf) : EntityEventArgs
{
    public NeedDatum Need = need;
    public EntityUid Examinee = examinee;
    public bool IsSelf = isSelf;
    public List<string> AdditionalInfoLines = new();

    public void AppendAdditionalInfoLines(ref string baseString)
    {
        foreach (var line in AdditionalInfoLines)
        {
            baseString += line + "\n";
        }
    }

    public void AddPercentBuff(string kind, string text, float modifier)
    {
        if (Math.Abs(modifier - 1.0f) < 0.001f)
            return;
        var percent = $"{(modifier - 1.0f) * 100.0f:+0;-0}%";
        if (modifier < 1.0f)
        {
            AdditionalInfoLines.Add(
                Loc.GetString(
                    "examinable-need-effect-buff",
                    ("kind", kind),
                    ("amount", percent),
                    ("text", text)));
        }
        else
        {
            AdditionalInfoLines.Add(
                Loc.GetString(
                    "examinable-need-effect-debuff",
                    ("kind", kind),
                    ("amount", percent),
                    ("text", text)));
        }
    }
    public void AddRawBuff(string kind, string text, bool isBuff)
    {
        if (isBuff)
        {
            AdditionalInfoLines.Add(
                Loc.GetString(
                    "examinable-need-effect-buff-custom",
                    ("kind", kind),
                    ("text", text)));
        }
        else
        {
            AdditionalInfoLines.Add(
                Loc.GetString(
                    "examinable-need-effect-debuff-custom",
                    ("kind", kind),
                    ("text", text)));
        }
    }
}

public struct NeedThresholdUpdateResult(NeedThreshold oldThreshold, NeedThreshold newThreshold)
{
    public NeedThreshold OldThreshold = oldThreshold;
    public NeedThreshold NewThreshold = newThreshold;
    public bool Changed => OldThreshold != NewThreshold;
}

