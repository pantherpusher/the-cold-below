using Content.Shared.Chat;
using Content.Shared.Radio;

namespace Content.Server._Coyote;

/// <summary>
/// This is the event raised when a roleplay incentive action is taken.
/// </summary>
public sealed class GetRoleplayIncentiveModifier(
    EntityUid source,
    float multiplier,
    float additive
    )
    : EntityEventArgs
{
    public EntityUid Source = source;
    public float Multiplier = multiplier;
    public float Additive = additive;

    /// <summary>
    /// This is used to modify the roleplay incentive multiplier and additive values.
    /// </summary>
    public void Modify(float multiplier, float additive)
    {
        Multiplier *= multiplier;
        Additive += additive;
    }
}
