namespace Content.Server._Coyote.CoolIncentives;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent]
public sealed partial class CoolchefComponent : Component
{
    /// <summary>
    /// This is a multiplier value (default is 1f) used to modify cooking-related calculations.
    /// </summary>
    [DataField("multiplier")]
    public float Multiplier = 1f;
}
