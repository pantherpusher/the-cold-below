using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Shared._Coyote.RolePlayIncentiveShared;
using Content.Shared.Alert;
using Content.Shared.Examine;
using Content.Shared.Humanoid;
using Content.Shared.IdentityManagement;
using Content.Shared.Movement.Systems;
using Content.Shared.Nutrition.Components;
using Content.Shared.Overlays;
using Content.Shared.Rejuvenate;
using Content.Shared.StatusIcon;
using Content.Shared.Verbs;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared._Coyote.Needs;

/// <summary>
/// This handles your needs.
/// </summary>
public abstract class SharedNeedsSystem : EntitySystem
{
    [Dependency] private readonly MovementSpeedModifierSystem _movement = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly AlertsSystem _alerts = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly IEntityManager _entMan = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly SharedHumanoidAppearanceSystem _humanoid = default!;
    [Dependency] private readonly ExamineSystemShared _examineSystem = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<NeedsComponent, ComponentInit>(OnInit);
        SubscribeLocalEvent<NeedsComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<NeedsComponent, RefreshMovementSpeedModifiersEvent>(OnRefreshMovespeed);
        SubscribeLocalEvent<NeedsComponent, RejuvenateEvent>(OnRejuvenate);
        SubscribeLocalEvent<NeedsComponent, GetRoleplayIncentiveModifier>(OnGetRoleplayIncentive);
        SubscribeLocalEvent<NeedsComponent, GetVerbsEvent<ExamineVerb>>(OnGetExamineVerbs);
    }

    #region Event Handlers
    private void OnInit(EntityUid uid, NeedsComponent component, ComponentInit args)
    {
        LoadNeeds(uid, component);
    }

    private void OnShutdown(EntityUid uid, NeedsComponent component, ComponentShutdown args)
    {
        foreach (var need in component.Needs.Values)
        {
            _alerts.ClearAlertCategory(uid, need.AlertCategory);
        }
    }

    private void OnRefreshMovespeed(EntityUid uid,
        NeedsComponent component,
        ref RefreshMovementSpeedModifiersEvent args)
    {
        foreach (var need in component.Needs.Values)
        {
            need.ApplyMovementSpeedModifier(ref args);
        }
    }

    private void OnRejuvenate(EntityUid uid, NeedsComponent component, ref RejuvenateEvent args)
    {
        component.Needs.Clear();
        LoadNeeds(uid, component);
        InitializeNextUpdate(uid, component);
        UpdateMovespeed(uid, component);
        UpdateAlerts(uid, component);
    }

    private void OnGetRoleplayIncentive(EntityUid uid, NeedsComponent component, ref GetRoleplayIncentiveModifier args)
    {
        foreach (var need in component.Needs.Values)
        {
            need.ModifyRpiEvent(ref args);
        }
    }

    #endregion
    #region Examine stuff
   /// <summary>
    /// Gets the description for the current threshold, if any
    /// </summary>
    public string GetExamineText(
        EntityUid examiner,
        EntityUid examinee,
        NeedDatum need,
        string species,
        bool showNumbers,
        bool showExtendedInfo)
    {
        var stringOut = string.Empty;
        var isSelf = examiner == examinee;
        var header = Loc.GetString(
            "examinable-need-header",
            ("color", need.NeedColor.ToHex()),
            ("needname", need.NeedName));
        stringOut += header + "\n";
        var meme = need.CurrentThreshold == NeedThreshold.Low
                   && IoCManager.Resolve<IRobustRandom>().Prob(0.05f);
        if (!showExtendedInfo)
        {
            var locStr = $"examinable-need-{need.NeedType.ToString().ToLower()}-{need.CurrentThreshold.ToString().ToLower()}";
            if (meme)
                locStr += "-meme";
            if (isSelf)
            {
                locStr += "-self";
            }
            var locThing = Loc.GetString(
                locStr,
                ("entity", Identity.Entity(examinee, IoCManager.Resolve<IEntityManager>())));
            stringOut += locThing + "\n";
            return stringOut; // suckit
        }

        // self examine, far more detailed!
        var locStrSelf = $"examinable-need-{need.NeedType.ToString().ToLower()}-{need.CurrentThreshold.ToString().ToLower()}-self";
        if (meme)
            locStrSelf += "-meme";
        var textOutSelf = Loc.GetString(locStrSelf);
        stringOut += textOutSelf + "\n";
        if (showNumbers)
        {
            string textOutNumbers;
            if (isSelf)
            {
                textOutNumbers = Loc.GetString(
                    "examinable-need-hunger-numberized-self",
                    ("current", (int) need.CurrentValue),
                    ("max", (int) need.MaxValue));
            }
            else
            {
                textOutNumbers = Loc.GetString(
                    "examinable-need-hunger-numberized",
                    ("entity", Identity.Entity(examinee, IoCManager.Resolve<IEntityManager>())),
                    ("current", (int) need.CurrentValue),
                    ("max", (int) need.MaxValue));
            }

            stringOut += textOutNumbers + "\n";
        }

        // Now, add in the time until next threshold change, if applicable
        string needChungus;
        if (need.CurrentThreshold == NeedThreshold.Critical)
        {
            // we need the entity's species, if we can get it
            // for meme reasons (Wizard needs food badly)
            needChungus = Loc.GetString(
                $"examinable-need-{need.NeedType.ToString().ToLower()}-timeleft-critical",
                ("creature", species));
            stringOut += needChungus + "\n";
        }
        else
        {
            var timeTillnext = need.GetTimeFromNowToNextThreshold();
            var timeString = need.Time2String(timeTillnext);
            if (isSelf)
            {
                needChungus = Loc.GetString(
                    $"examinable-need-{need.NeedType.ToString().ToLower()}-timeleft-{need.CurrentThreshold.ToString().ToLower()}-self");
            }
            else
            {
                needChungus = Loc.GetString(
                    $"examinable-need-{need.NeedType.ToString().ToLower()}-timeleft-{need.CurrentThreshold.ToString().ToLower()}",
                    ("entity", Identity.Entity(examinee, IoCManager.Resolve<IEntityManager>())));
            }

            stringOut += needChungus + "\n";
            stringOut += timeString + "\n";

            var timeTillStarve = need.GetTimeToMinValue();
            var timeStringStarve = need.Time2String(timeTillStarve);
            if (isSelf)
            {
                needChungus = Loc.GetString(
                    $"examinable-need-{need.NeedType.ToString().ToLower()}-timeleft-tillcritical-self");
            }
            else
            {
                needChungus = Loc.GetString(
                    $"examinable-need-{need.NeedType.ToString().ToLower()}-timeleft-tillcritical",
                    ("entity", Identity.Entity(examinee, IoCManager.Resolve<IEntityManager>())));
            }
            stringOut += needChungus + "\n";
            stringOut += timeStringStarve + "\n";

        }
        var needMods = need.GetBuffDebuffList(stringOut);
        // ANYTHING ELSE YOU WANT TO ADD?
        var ev = new NeedExamineInfoEvent(
            need,
            examinee,
            isSelf);
        RaiseLocalEvent(examinee, ev);
        ev.AppendAdditionalInfoLines(ref stringOut);
        return stringOut;
    }
    private void OnGetExamineVerbs(EntityUid uid, NeedsComponent needy, GetVerbsEvent<ExamineVerb> args)
    {
        var detailsRange = _examineSystem.IsInDetailsRange(args.User, uid);

        var verb = new ExamineVerb()
        {
            Act = () =>
            {
                var info = string.Empty;

                if (!needy.Ready
                    || needy.Needs.Count == 0)
                    return;
                var examinerIsSelf = args.User == args.Target;

                var coolMedicalHud = EntityManager.HasComponent<ShowHealthBarsComponent>(args.User);
                var showExtendedInfo = coolMedicalHud || examinerIsSelf;
                var showNumbers = coolMedicalHud;
                // get the mob's species, if possible
                // (for the "X is starving to death" examine text)
                var species = "Critter"; // default fallback
                if (_entMan.TryGetComponent(args.Target, out HumanoidAppearanceComponent? humanoid))
                {
                    species = _humanoid.GetSpeciesRepresentation(humanoid.Species, humanoid.CustomSpecieName);
                }
                foreach (var need in needy.Needs.Values)
                {
                    if (!needy.VisibleNeeds.TryGetValue(need.NeedType, out var visibility))
                        continue;
                    if (visibility == NeedExamineVisibility.None)
                        continue;
                    if (visibility == NeedExamineVisibility.Owner
                        && !examinerIsSelf)
                        continue;
                    var line = GetExamineText(
                        args.User,
                        args.Target,
                        need,
                        species,
                        showNumbers,
                        showExtendedInfo);
                    if (string.IsNullOrEmpty(line))
                        continue;
                    info += line + "\n";
                }
                var markup = FormattedMessage.FromMarkupPermissive(info);
                _examineSystem.SendExamineTooltip(
                    args.User,
                    uid,
                    markup,
                    false,
                    false);
            },
            Text = Loc.GetString("examinable-need-verb-text"),
            Category = VerbCategory.Examine,
            Disabled = !detailsRange,
            Message = detailsRange ? null : Loc.GetString("examinable-need-verb-disabled"),
            Icon = new SpriteSpecifier.Texture(new("/Textures/Interface/NavMap/beveled_triangle.png"))
        };

        args.Verbs.Add(verb);
    }

    #endregion
    #region Helpers
    private void LoadNeeds(EntityUid uid, NeedsComponent component)
    {
        component.Needs.Clear();
        foreach (var need in component.NeedPrototypes)
        {
            _prototype.TryIndex(need, out var proto);
            if (proto == null)
                continue;
            var protoKind = Enum.Parse<NeedType>(proto.NeedKind);
            var datum = new NeedDatum(proto);
            component.Needs[protoKind] = datum;
        }
        InitializeNextUpdate(uid, component);
        _movement.RefreshMovementSpeedModifiers(uid);
        component.Ready = true;
    }

    private void InitializeNextUpdate(EntityUid uid, NeedsComponent component)
    {
        var curTime = _timing.CurTime;
        var shortestDelay = TimeSpan.MaxValue;
        foreach (var need in component.Needs.Values)
        {
            if (need.UpdateRateSeconds < shortestDelay)
            {
                shortestDelay = need.UpdateRateSeconds;
            }
            need.NextUpdateTime = curTime + need.UpdateRateSeconds;
        }
        component.MinUpdateTime = shortestDelay;
        component.NextUpdateTime = curTime + component.MinUpdateTime;
    }
    #endregion

    #region Generic Need Helpers
    public bool TryGetStatusIconPrototype(
        EntityUid uid,
        NeedType kind,
        NeedsComponent component,
        [NotNullWhen(true)] out SatiationIconPrototype? prototype)
    {
        if (!HasComp<NeedsComponent>(uid))
        {
            prototype = null;
            return false;
        }
        var need = component.Needs.GetValueOrDefault(kind);
        if (need == null)
        {
            prototype = null;
            return false;
        }
        if (need.GetCurrentStatusIcon(out var iconId)
            && !string.IsNullOrEmpty(iconId))
        {
            return IoCManager.Resolve<IPrototypeManager>()
                .TryIndex(
                    iconId,
                    out prototype);
        }

        prototype = null;
        return false;
    }
    /// <summary>
    /// Gets the current level of a specific need for an entity.
    /// </summary>
    public float? TryGetNeedLevel(
        EntityUid uid,
        NeedType needType,
        NeedsComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return null;

        if (component.Needs.TryGetValue(needType, out var need))
        {
            return need.CurrentValue;
        }
        return null;
    }

    /// <summary>
    /// Gets the current threshold of a specific need for an entity.
    /// </summary>
    public NeedThreshold? TryGetNeedThreshold(
        EntityUid uid,
        NeedType needType,
        NeedsComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return null;

        if (component.Needs.TryGetValue(needType, out var need))
        {
            return need.CurrentThreshold;
        }
        return null;
    }

    /// <summary>
    /// Gets the minimum threshold of a specific threshold of a need for an entity.
    /// </summary>
    public float? TryGetNeedMinThreshold(
        EntityUid uid,
        NeedType needType,
        NeedThreshold threshold,
        NeedsComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return null;
        if (component.Needs.TryGetValue(needType, out var need))
        {
            return need.GetValueForThreshold(threshold);
        }
        return null;
    }

    /// <summary>
    /// Modifies the current level of a specific need for an entity.
    /// </summary>
    public bool TryModifyNeedLevel(
        EntityUid uid,
        NeedType needType,
        float amount,
        NeedsComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return false;
        if (component.Needs.TryGetValue(needType, out var need))
        {
            need.ModifyCurrentValue(amount);
            UpdateEverythingIfNeeded(uid, component);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Tries to set the current level of a specific need for an entity.
    /// </summary>
    public bool TrySetNeedLevel(
        EntityUid uid,
        NeedType needType,
        float amount,
        NeedsComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return false;
        if (component.Needs.TryGetValue(needType, out var need))
        {
            need.SetCurrentValue(amount);
            UpdateEverythingIfNeeded(uid, component);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Sets the current level of a specific need to the minimum value of a specific threshold for an entity.
    /// </summary>
    public bool SetNeedToThreshold(
        EntityUid uid,
        NeedType needType,
        NeedThreshold threshold,
        NeedsComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return false;
        if (component.Needs.TryGetValue(needType, out var need))
        {
            var minValue = need.GetValueForThreshold(threshold);
            need.SetCurrentValue(minValue);
            UpdateEverythingIfNeeded(uid, component);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if the entity's specific need is below a certain threshold.
    /// </summary>
    public bool IsBelowThreshold(
        EntityUid uid,
        NeedType needType,
        NeedThreshold threshold,
        NeedsComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return false;
        if (component.Needs.TryGetValue(needType, out var need))
        {
            return need.IsBelowThreshold(threshold);
        }
        return false;
    }

    /// <summary>
    /// Does the entity have a specific need?
    /// </summary>
    public bool HasNeed(
        EntityUid uid,
        NeedType needType,
        NeedsComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return false;
        return component.Needs.ContainsKey(needType);
    }
    #endregion

    #region Hunger Helpers
    /// <summary>
    /// Gets the Hunger level of an entity.
    /// </summary>
    public float? GetHunger(
        EntityUid uid,
        NeedsComponent? component = null)
    {
        return TryGetNeedLevel(
            uid,
            NeedType.Hunger,
            component);
    }

    /// <summary>
    /// Does the entity use Hunger as a need?
    /// </summary>
    public bool UsesHunger(EntityUid uid, NeedsComponent? component = null)
    {
        return HasNeed(
            uid,
            NeedType.Hunger,
            component);
    }

    /// <summary>
    /// Modifies the Hunger level of an entity.
    /// </summary>
    public bool ModifyHunger(EntityUid uid, float amount, NeedsComponent? component = null)
    {
        return TryModifyNeedLevel(
            uid,
            NeedType.Hunger,
            amount,
            component);
    }

    /// <summary>
    /// Sets the Hunger level of an entity.
    /// </summary>
    public bool SetHunger(EntityUid uid, float amount, NeedsComponent? component = null)
    {
        return TrySetNeedLevel(
            uid,
            NeedType.Hunger,
            amount,
            component);
    }

    public bool TryGetHungerStatusIconPrototype(EntityUid uid,
        [NotNullWhen(true)] out SatiationIconPrototype? prototype,
        NeedsComponent? component = null)
    {
        if (!Resolve(uid, ref component))
        {
            prototype = null;
            return false;
        }

        return TryGetStatusIconPrototype(
            uid,
            NeedType.Hunger,
            component,
            out prototype);
    }

    /// <summary>
    /// Hunger is below this threshold
    /// </summary>
    public bool HungerIsBelowThreshold(EntityUid uid, NeedThreshold threshold, NeedsComponent? component = null)
    {
        return IsBelowThreshold(
            uid,
            NeedType.Hunger,
            threshold,
            component);
    }

    /// <summary>
    /// Gets the current threshold of Hunger for an entity.
    /// </summary>
    public NeedThreshold? GetHungerThreshold(EntityUid uid, NeedsComponent? component = null)
    {
        return TryGetNeedThreshold(
            uid,
            NeedType.Hunger,
            component);
    }

    /// <summary>
    /// Gets the minimum threshold of a specific threshold of Hunger for an entity.
    /// </summary>
    public float? GetHungerMinThreshold(EntityUid uid, NeedThreshold threshold, NeedsComponent? component = null)
    {
        return TryGetNeedMinThreshold(
            uid,
            NeedType.Hunger,
            threshold,
            component);
    }

    /// <summary>
    /// Sets the current level of Hunger to the minimum value of a specific threshold for an entity.
    /// </summary>
    public bool SetHungerToThreshold(
        EntityUid uid,
        NeedThreshold threshold = NeedThreshold.Satisfied,
        NeedsComponent? component = null)
    {
        return SetNeedToThreshold(
            uid,
            NeedType.Hunger,
            threshold,
            component);
    }
    #endregion

    #region Thirst Helpers
    /// <summary>
    /// Gets the Thirst level of an entity.
    /// </summary>
    public float? GetThirst(EntityUid uid, NeedsComponent? component = null)
    {
        return TryGetNeedLevel(
            uid,
            NeedType.Thirst,
            component);
    }

    /// <summary>
    /// Modifies the Thirst level of an entity.
    /// </summary>
    public bool ModifyThirst(EntityUid uid, float amount, NeedsComponent? component = null)
    {
        return TryModifyNeedLevel(
            uid,
            NeedType.Thirst,
            amount,
            component);
    }

    /// <summary>
    /// Sets the Thirst level of an entity.
    /// </summary>
    public bool SetThirst(EntityUid uid, float amount, NeedsComponent? component = null)
    {
        return TrySetNeedLevel(
            uid,
            NeedType.Thirst,
            amount,
            component);
    }

    /// <summary>
    /// Does the entity use Thirst as a need?
    /// </summary>
    public bool UsesThirst(EntityUid uid, NeedsComponent? component = null)
    {
        return HasNeed(
            uid,
            NeedType.Thirst,
            component);
    }

    /// <summary>
    /// Thirst is below this threshold
    /// </summary>
    public bool ThirstIsBelowThreshold(EntityUid uid, NeedThreshold threshold, NeedsComponent? component = null)
    {
        return IsBelowThreshold(
            uid,
            NeedType.Thirst,
            threshold,
            component);
    }

    public bool TryGetThirstStatusIconPrototype(EntityUid uid,
        [NotNullWhen(true)] out SatiationIconPrototype? prototype,
        NeedsComponent? component = null)
    {
        if (!Resolve(uid, ref component))
        {
            prototype = null;
            return false;
        }

        return TryGetStatusIconPrototype(
            uid,
            NeedType.Thirst,
            component,
            out prototype);
    }

    /// <summary>
    /// Gets the current threshold of Thirst for an entity.
    /// </summary>
    public NeedThreshold? GetThirstThreshold(EntityUid uid, NeedsComponent? component = null)
    {
        return TryGetNeedThreshold(
            uid,
            NeedType.Thirst,
            component);
    }

    /// <summary>
    /// Sets the current level of Thirst to the minimum value of a specific threshold for an entity.
    /// </summary>
    public bool SetThirstToThreshold(
        EntityUid uid,
        NeedThreshold threshold = NeedThreshold.Satisfied,
        NeedsComponent? component = null)
    {
        return SetNeedToThreshold(
            uid,
            NeedType.Thirst,
            threshold,
            component);
    }

    /// <summary>
    /// Gets the minimum threshold of a specific threshold of Thirst for an entity.
    /// </summary>
    public float? GetThirstMinThreshold(EntityUid uid, NeedThreshold threshold, NeedsComponent? component = null)
    {
        return TryGetNeedMinThreshold(
            uid,
            NeedType.Thirst,
            threshold,
            component);
    }
    #endregion

    #region Updates
    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        var curTime = _timing.CurTime;
        var query = EntityQueryEnumerator<NeedsComponent>();
        while (query.MoveNext(out var uid, out var component))
        {
            if (component.Needs.Count == 0)
                continue;
            if (!component.Ready)
                continue;
            if (component.NextUpdateTime > curTime)
                continue;

            var deltaSeconds = (float) (curTime - (component.NextUpdateTime - component.MinUpdateTime)).TotalSeconds;
            component.NextUpdateTime = curTime + component.MinUpdateTime;

            foreach (var need in component.Needs.Values)
            {
                need.Decay(deltaSeconds);
            }
            UpdateEverythingIfNeeded(uid, component);
        }
    }

    private void UpdateEverything(EntityUid uid, NeedsComponent component)
    {
        UpdateMovespeed(uid, component);
        UpdateAlerts(uid, component);
    }

    private void UpdateEverythingIfNeeded(EntityUid uid, NeedsComponent component)
    {
        if (component.Needs.Values.Any(need => need.UpdateCurrentThreshold().Changed))
        {
            UpdateEverything(uid, component);
        }
    }

    private void UpdateMovespeed(EntityUid uid, NeedsComponent component)
    {
        _movement.RefreshMovementSpeedModifiers(uid);
    }

    private void UpdateAlerts(EntityUid uid, NeedsComponent component)
    {
        foreach (var need in component.Needs.Values)
        {
            var alertProto = need.GetCurrentAlert();
            if (!_alerts.TryGet(alertProto, out var _))
            {
                var alertCat = need.AlertCategory;
                _alerts.ClearAlertCategory(uid, alertCat);
                continue;
            }
            _alerts.ShowAlert(uid, alertProto);
        }
    }
    #endregion

}
