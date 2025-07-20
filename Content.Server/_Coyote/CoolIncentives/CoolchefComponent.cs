namespace Content.Server._Coyote.CoolIncentives;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent]
public sealed partial class CoolchefComponent : Component
{
    /// <summary>
    /// This is used to track the number of times a player has cooked a meal.
    /// </summary>
    [DataField("multiplier")]
    public float Multiplier = 1f;
}
