using Content.Shared.Damage;

namespace Content.Server._Coyote.HealingBank;

/// <summary>
/// will eventually be used to store healing for an entity and recharge it over time
/// </summary>
[RegisterComponent]
public sealed partial class HealingBankComponent : Component
{
    /// <summary>
    /// How much healing the entity has stored.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public DamageSpecifier StoredHealing = new();

    /// <summary>
    /// Base rate of healing recharge per second.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public DamageSpecifier HealingRechargeRate = new();

    /// <summary>
    /// Maximum amount of healing that can be stored.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public DamageSpecifier MaxStoredHealing = new();
}
