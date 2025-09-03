using Content.Shared._Coyote;
using Robust.Shared.Prototypes;

namespace Content.Shared._Coyote.Needs;

/// <summary>
/// The needs component is a marker component for entities that have needs such as hunger, thirst, and such
/// holds a list of need 'datums' that, honestly, do most of the work. just dont call it needy
/// </summary>
[RegisterComponent]
public sealed partial class NeedsComponent : Component
{
    /// <summary>
    /// The set of datums that this entity has for needs
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public Dictionary<NeedType, NeedDatum> Needs = new();

    /// <summary>
    /// Is the component initialized?
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public bool Ready = false;

    /// <summary>
    /// The shortest amount of time between need updates
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan MinUpdateTime = TimeSpan.Zero;

    /// <summary>
    /// The next time the needs should update
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan NextUpdateTime = TimeSpan.Zero;

    /// <summary>
    /// The actual way that prototypes load stuff into the needs dictionary
    /// Mainly cus I dont know how yaml works, so im gonna do it MY WAY
    /// </summary>
    [DataField("needs")]
    public List<ProtoId<NeedPrototype>> NeedPrototypes = new()
    {
        "NeedHungerDefault",
        "NeedThirstDefault",
    };

    /// <summary>
    /// Dictionary of which needs are visible, and to whom they are visible.
    /// </summary>
    [DataField]
    public Dictionary<NeedType, NeedExamineVisibility> VisibleNeeds = new()
    {
        { NeedType.Hunger, NeedExamineVisibility.Owner },
        { NeedType.Thirst, NeedExamineVisibility.Owner },
    };
}
/// <summary>
/// Visibility settings for a need on examine.
/// </summary>
public enum NeedExamineVisibility : byte
{
    /// <summary>
    /// The need is not shown on examine.
    /// </summary>
    None = 0,
    /// <summary>
    /// The need is shown on examine to everyone.
    /// </summary>
    All = 1,
    /// <summary>
    /// The need is shown on examine only to the owner.
    /// Can be overridden by ghosts, admins, and people with certain event responses.
    /// </summary>
    Owner = 2,
}
