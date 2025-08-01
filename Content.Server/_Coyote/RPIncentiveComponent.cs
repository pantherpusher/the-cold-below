namespace Content.Server._Coyote;

/// <summary>
/// Hi! This is the RP incentive component.
/// This will track the actions a player does, and adjust some paywards
/// for them once if they do those things, sometimes!
/// </summary>
[RegisterComponent]
public sealed partial class RoleplayIncentiveComponent : Component
{
    /// <summary>
    /// The actions that have taken place.
    /// </summary>
    [DataField]
    public List<RoleplayAction> ActionsTaken = new();

    /// <summary>
    /// The last time the system checked for actions, for paywards.
    /// </summary>
    [DataField]
    public DateTime LastCheck = DateTime.MinValue;

    /// <summary>
    /// The next time the system will check for actions, for paywards.
    /// </summary>
    [DataField]
    public TimeSpan NextPayward = TimeSpan.Zero;

    /// <summary>
    /// Interval between paywards.
    /// </summary>
    [DataField]
    public TimeSpan PaywardInterval = TimeSpan.FromMinutes(20); // TimeSpan.FromMinutes(15);

    /// <summary>
    /// Interval between paywards when offline.
    /// hey guess what doesnt work? this thing!
    /// </summary>
    [DataField]
    public TimeSpan PaywardIntervalOffline = TimeSpan.FromMinutes(30); // TimeSpan.FromMinutes(15);

    [ViewVariables(VVAccess.ReadWrite)]
    public int TaxBracket1Payout = 300;
    [ViewVariables(VVAccess.ReadWrite)]
    public int TaxBracket2Payout = 200;
    [ViewVariables(VVAccess.ReadWrite)]
    public int TaxBracket3Payout = 100;
    [ViewVariables(VVAccess.ReadWrite)]
    public int TaxBracketRestPayout = 80;
    [ViewVariables(VVAccess.ReadWrite)]
    public int TaxBracketPayoutOverride = -1; // -1 means no override, use the default payouts
    [ViewVariables(VVAccess.ReadWrite)]
    public string TaxZZZCurrentActiveBracket = "TaxBracket1"; // purely for debugging purposes, to see which bracket is active

}
