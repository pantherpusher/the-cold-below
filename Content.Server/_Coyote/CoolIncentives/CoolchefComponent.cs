namespace Content.Server._Coyote.CoolIncentives;

/// <summary>
/// Represents a component that provides role-playing (RP) incentive multipliers
/// for food service workers, encouraging engagement in cooking activities.
/// </summary>
[Virtual]
public abstract partial class RoleplayIncentiveModifierComponent : Component
{
    /// <summary>
    /// This is a multiplier value (default is 1f) used to modify cooking-related calculations.
    /// </summary>
    public float Multiplier = 1.25f;
}

[RegisterComponent]
public sealed partial class CoolChefComponent : RoleplayIncentiveModifierComponent
{
    public new float Multiplier = 1.50f;
}

[RegisterComponent]
public sealed partial class CoolPirateComponent : RoleplayIncentiveModifierComponent
{
    public new float Multiplier = 1.45f;
}

[RegisterComponent]
public sealed partial class CoolStationRepComponent : RoleplayIncentiveModifierComponent
{
    public new float Multiplier = 5.5f;
}

[RegisterComponent]
public sealed partial class CoolStationTrafficControllerComponent : RoleplayIncentiveModifierComponent
{
    public new float Multiplier = 5f;
}

[RegisterComponent]
public sealed partial class CoolStationDirectorOfCareComponent : RoleplayIncentiveModifierComponent
{
    public new float Multiplier = 5f;
}

[RegisterComponent]
public sealed partial class CoolSheriffComponent : RoleplayIncentiveModifierComponent
{
    public new float Multiplier = 6f;
}

[RegisterComponent]
public sealed partial class CoolNfsdComponent : RoleplayIncentiveModifierComponent
{
    public new float Multiplier = 4.5f;
}

