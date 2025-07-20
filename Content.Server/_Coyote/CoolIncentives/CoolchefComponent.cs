namespace Content.Server._Coyote.CoolIncentives;

/// <summary>
/// Represents a component that provides role-playing (RP) incentive multipliers
/// for food service workers, encouraging engagement in cooking activities.
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
