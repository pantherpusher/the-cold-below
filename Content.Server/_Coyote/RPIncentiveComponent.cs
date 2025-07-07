using Content.Shared._Coyote;
using Robust.Shared.GameStates;

namespace Content.Server._Coyote;

/// <summary>
/// Hi! This is the RP incentive component.
/// This will track the actions a player does, and adjust some paywards
/// for them once if they do those things, sometimes!
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class RoleplayIncentiveComponent : Component
{
    /// <summary>
    /// The actions that have taken place.
    /// </summary>
    [DataField]
    public HashSet<RoleplayAction> ActionsTaken = new();

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
    public TimeSpan PaywardInterval = TimeSpan.FromSeconds(5); // TimeSpan.FromMinutes(60);

    /// <summary>
    /// Interval between paywards when offline.
    /// </summary>
    [DataField]
    public TimeSpan PaywardIntervalOffline = TimeSpan.FromSeconds(10); // TimeSpan.FromMinutes(15);

    /// <summary>
    /// The amount of payward to give for each action.
    /// </summary>
    [DataField]
    public float PaywardScalar = 0.001f;

}
