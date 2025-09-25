using Content.Shared._Coyote.Needs;
using Content.Shared.Movement.Systems;

namespace Content.Shared._Coyote;

/// <summary>
/// This handles...
/// </summary>
public sealed class OverweightTraitSystem : EntitySystem
{
    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<OverweightTraitComponent, ComponentInit>(OnInit);
        SubscribeLocalEvent<OverweightTraitComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<OverweightTraitComponent, RefreshMovementSpeedModifiersEvent>(OnRefreshMovespeed);
    }
}
