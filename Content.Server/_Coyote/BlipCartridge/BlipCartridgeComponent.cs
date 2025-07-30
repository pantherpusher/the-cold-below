using Robust.Shared.Prototypes;

namespace Content.Server._Coyote.BlipCartridge;

/// <summary>
/// This component is used to add a radar blip for your PDA when the Blip Cartridge is equipped!
/// Great for dying in the middle of nowhere and having pirates ransom your body!
/// </summary>
[RegisterComponent]
public sealed partial class BlipCartridgeComponent : Component
{
    /// <summary>
    /// Default preset for the blip cartridge.
    /// </summary>
    [DataField]
    public EntProtoId DefaultPreset { get; set; } = "BlipPresetCivilian";

    /// <summary>
    /// Current preset for the blip cartridge.
    /// </summary>
    [DataField]
    public EntProtoId CurrentPreset { get; set; } = "BlipPresetCivilian";

    // stored blip data for like when the cartridge is removed and for to be re-added later
    /// <summary>
    /// Color Table Set for the blip.
    /// </summary>
    [DataField]
    public EntProtoId BlipColor { get; set; } = "BlipColorRed";

    /// <summary>
    /// Shape Table Set for the blip.
    /// </summary>
    [DataField]
    public EntProtoId BlipShape { get; set; } = "BlipShapeCircle";

    /// <summary>
    /// Scale of the blip.
    /// </summary>
    [DataField]
    public float Scale { get; set; } = 1f;

    /// <summary>
    /// Whether this blip is enabled and should be shown on radar.
    /// </summary>
    [DataField]
    public bool Enabled { get; set; } = true;

    // Settings that can setting for it
    /// <summary>
    /// A list that maps color names to their corresponding color values.
    /// prototypes
    /// </summary>
    public List<EntProtoId> ColorTable = new()
    {
        "BlipColorRed",
        "BlipColorGreen",
        "BlipColorBlue",
        "BlipColorYellow",
        "BlipColorPurple",
        "BlipColorGold",
        "BlipColorWhite",
        "BlipColorCyan",
    };

    /// <summary>
    /// A list that maps shape names to their corresponding shape values.
    /// proots
    /// </summary>
    public List<EntProtoId> ShapeTable = new()
    {
        "BlipShapeCircle",
        "BlipShapeSquare",
        "BlipShapeTriangle",
        "BlipShapeDiamond",
        "BlipShapeHexagon",
        "BlipShapeStar",
        "BlipShapeArrow",
    };

    /// <summary>
    /// Available blip presets for the cartridge.
    /// </summary>
    public List<EntProtoId> Presets = new()
    {
        "BlipPresetCivilian",
        "BlipPresetMercenary",
        "BlipPresetCommand",
        "BlipPresetPirate",
        "BlipPresetMedical",
        "BlipPresetEngineering",
        "BlipPresetSecurity",
        "BlipPresetScience",
        "BlipPresetSupply",
    };
}

/// <summary>
///     Component attached to the PDA a BlipCartridge cartridge is inserted into for interaction handling
/// </summary>
[RegisterComponent]
public sealed partial class BlipCartridgeInteractionComponent : Component;
