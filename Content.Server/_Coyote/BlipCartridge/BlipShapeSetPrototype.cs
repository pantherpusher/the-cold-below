using Robust.Shared.Prototypes;

namespace Content.Server._Coyote.BlipCartridge;

/// <summary>
/// This is a prototype for...
/// </summary>
[Prototype]
public sealed partial class BlipShapeSetPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; } = default!;

    /// <summary>
    /// The name of the blip shape set.
    /// </summary>
    [DataField]
    public string Name { get; set; } = default!;

    /// <summary>
    /// The shape of the blip on the radar.
    /// MUST have a name identical to RadarBlipShape enum value, or all is lost.
    /// </summary>
    [DataField]
    public string Shape { get; set; } = "Circle";
}
