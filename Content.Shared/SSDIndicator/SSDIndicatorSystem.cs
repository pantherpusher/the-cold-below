using Content.Shared.Bed.Sleep;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Shared.SSDIndicator;

/// <summary>
///     Handle changing player SSD indicator status
/// </summary>
public sealed class SSDIndicatorSystem : EntitySystem
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    private bool _icSsdSleep;
    private float _icSsdSleepTime;
    private float _jobReopenMinutes = 120f;

    public override void Initialize()
    {
        SubscribeLocalEvent<SSDIndicatorComponent, PlayerAttachedEvent>(OnPlayerAttached);
        SubscribeLocalEvent<SSDIndicatorComponent, PlayerDetachedEvent>(OnPlayerDetached);
        SubscribeLocalEvent<SSDIndicatorComponent, MapInitEvent>(OnMapInit);

        _cfg.OnValueChanged(
            CCVars.ICSSDSleep,
            obj => _icSsdSleep = obj,
            true);
        _cfg.OnValueChanged(
            CCVars.ICSSDSleepTime,
            obj => _icSsdSleepTime = obj,
            true);
        _cfg.OnValueChanged(
            CCVars.ICSSDJobReopenMinutes,
            obj => _jobReopenMinutes = obj,
            true);
    }

    private void OnPlayerAttached(EntityUid uid, SSDIndicatorComponent component, PlayerAttachedEvent args)
    {
        component.IsSSD = false;
        component.WentBraindeadAt = TimeSpan.Zero;

        // Removes force sleep and resets the time to zero
        if (_icSsdSleep)
        {
            component.FallAsleepTime = TimeSpan.Zero;
            if (component.ForcedSleepAdded) // Remove component only if it has been added by this system
            {
                EntityManager.RemoveComponent<ForcedSleepingComponent>(uid);
                component.ForcedSleepAdded = false;
            }
        }
        Dirty(uid, component);
    }

    private void OnPlayerDetached(EntityUid uid, SSDIndicatorComponent component, PlayerDetachedEvent args)
    {
        component.IsSSD = true;
        component.WentBraindeadAt = _timing.CurTime;

        // Sets the time when the entity should fall asleep
        if (_icSsdSleep)
        {
            component.FallAsleepTime = _timing.CurTime + TimeSpan.FromSeconds(_icSsdSleepTime);
        }
        Dirty(uid, component);
    }

    // Prevents mapped mobs to go to sleep immediately
    private void OnMapInit(EntityUid uid, SSDIndicatorComponent component, MapInitEvent args)
    {
        if (_icSsdSleep
            && component.IsSSD
            && component.FallAsleepTime == TimeSpan.Zero)
        {
            component.FallAsleepTime = _timing.CurTime + TimeSpan.FromSeconds(_icSsdSleepTime);
        }
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (!_icSsdSleep)
            return;

        var query = EntityQueryEnumerator<SSDIndicatorComponent>();

        while (query.MoveNext(out var uid, out var ssd))
        {
            // Forces the entity to sleep when the time has come
            if(ssd.IsSSD)
            {
                HandleForcedSleep(uid, ssd);
                HandleReopenJob(uid, ssd);
            }
        }
    }

    private void HandleForcedSleep(EntityUid uid, SSDIndicatorComponent comp)
    {
        if (!comp.PreventSleep
            && comp.FallAsleepTime <= _timing.CurTime // Frontier
            && !TerminatingOrDeleted(uid)
            && !HasComp<ForcedSleepingComponent>(
                uid)) // Don't add the component if the entity has it from another sources
        {
            EnsureComp<ForcedSleepingComponent>(uid);
            comp.ForcedSleepAdded = true;
        }
    }

    private void HandleReopenJob(EntityUid uid, SSDIndicatorComponent comp)
    {
        if (!comp.IsSSD
            || comp.WentBraindeadAt == TimeSpan.Zero)
            return;
        var curTime = _timing.CurTime;
        if (curTime < comp.WentBraindeadAt + TimeSpan.FromMinutes(_jobReopenMinutes))
            return;
        var ev = new SSDJobReopenEvent(uid);
        RaiseLocalEvent(uid, ev);
        comp.JobOpened = true;
    }
}

/// <summary>
/// Just tells the job system to try to reopen the job.
/// </summary>
public sealed class SSDJobReopenEvent : EntityEventArgs
{
    public EntityUid User { get; set; }

    public SSDJobReopenEvent(EntityUid user)
    {
        User = user;
    }
}
