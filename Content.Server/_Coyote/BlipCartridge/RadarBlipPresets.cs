using Robust.Shared.Prototypes;

namespace Content.Server._Coyote.BlipCartridge;

/// <summary>
/// This is a prototype for...
/// </summary>
[Prototype]
public sealed partial class RadarBlipPresetPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; } = default!;

    /// <summary>
    /// The name to display in the UI.
    /// </summary>
    [DataField]
    public string Name = "Cool Cute preset 2000";

    /// <summary>
    /// The color set prototype ID to use for this blip preset.
    /// </summary>
    [DataField]
    public EntProtoId ColorSet = "BlipPresetCivilian";

    /// <summary>
    /// The shape set prototype ID to use for this blip preset.
    /// </summary>
    [DataField]
    public EntProtoId ShapeSet = "BlipPresetCircle";

    /// <summary>
    /// The scale of the blip.
    /// </summary>
    [DataField]
    public float Scale = 1f;
}
