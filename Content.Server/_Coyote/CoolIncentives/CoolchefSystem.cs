// ReSharper disable InconsistentNaming
namespace Content.Server._Coyote.CoolIncentives;

/// <summary>
/// Handles roleplay incentive multipliers for food service workers.
/// This system modifies the roleplay incentive based on the multiplier
/// defined in the CoolchefComponent.
/// </summary>
public sealed class CoolchefSystem : EntitySystem
{
    [Dependency] private readonly IEntityManager EntManager = null!;
    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<RoleplayIncentiveComponent, GetRoleplayIncentiveModifier>(ModifyRPIncentive);
    }

    private void ModifyRPIncentive(Entity<RoleplayIncentiveComponent> ent, ref GetRoleplayIncentiveModifier args)
    {
        if (!EntManager.TryGetComponent(ent.Owner, out CoolchefComponent? chef))
            return;
        args.Modify(chef.Multiplier, 0f);
    }
}

