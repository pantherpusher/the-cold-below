using Content.Server._NF.Bank;
using Content.Server.Popups;
using Content.Shared.Popups;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Server._Coyote;

/// <summary>
/// This handles...
/// </summary>
public sealed class RoleplayIncentiveSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = null!;
    [Dependency] private readonly INetManager _net = null!;
    [Dependency] private readonly BankSystem _bank = null!;
    [Dependency] private readonly PopupSystem _popupSystem = null!;

    private const float GoodlenSpeaking = 50;
    private const float GoodlenWhispering = 50;
    private const float GoodlenEmoting = 50;
    private const float GoodlenQuickEmoting = 1;
    private const float GoodlenSubtling = 50;
    private const float GoodlenRadio = 50; // idk

    private const float GoodpplSpeaking = 1;
    private const float GoodpplWhispering = 1;
    private const float GoodpplEmoting = 1;
    private const float GoodpplQuickEmoting = 1;
    private const float GoodpplSubtling = 1;
    private const float GoodpplRadio = 0; // idk

    /// <inheritdoc/>
    public override void Initialize()
    {
        // get the component this thing is attached to
        SubscribeLocalEvent<RoleplayIncentiveComponent, ComponentInit>(OnComponentInit);
    }

    private void OnComponentInit(EntityUid uid, RoleplayIncentiveComponent component, ComponentInit args)
    {
        // set the next payward time
        component.NextPayward = _timing.CurTime + component.PaywardInterval;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<Server._Coyote.RoleplayIncentiveComponent>();
        while (query.MoveNext(out var uid, out var rpic))
        {
            if (_timing.CurTime < rpic.NextPayward)
                continue;
            if (_net.IsClient)
            {
                rpic.NextPayward = _timing.CurTime + rpic.PaywardIntervalOffline;
                return;
            }

            rpic.NextPayward = _timing.CurTime + rpic.PaywardInterval;
            UpdatePayward(uid, rpic);
        }
    }

    /// <summary>
    /// Goes through all the relevant actions taken and stored, judges them,
    /// And gives the player a payward if they did something good.
    /// It also checks for things like duplicate actions, if theres people around, etc.
    /// Basically if you do stuff, you get some pay for it!
    /// </summary>
    private void UpdatePayward(EntityUid uid, Server._Coyote.RoleplayIncentiveComponent rpic)
    {
        //first check if this rpic is actually on the uid
        if (!TryComp<Server._Coyote.RoleplayIncentiveComponent>(uid, out var incentive))
        {
            Log.Warning($"RoleplayIncentiveComponent not found on entity {uid}!");
            return;
        }

        // go through all the actions, and judge them into a cooler format
        var bestSay = 0f;
        var bestWhisper = 0f;
        var bestEmote = 0f;
        var bestQuickEmote = 0f;
        var bestSubtle = 0f;
        var bestRadio = 0f;
        // go through all the actions taken, sort and judge them
        foreach (var action in incentive.ActionsTaken)
        {
            if (action.Judgement > 0)
            {
                // if the action has already been judged, skip it
                continue;
            }
            JudgeAction(action, out var judgement);
            // slot it into the best action for that type
            switch (action.Action)
            {
                case RoleplayActs.Speaking:
                    if (judgement > bestSay)
                    {
                        bestSay = judgement;
                    }

                    break;
                case RoleplayActs.Whispering:
                    if (judgement > bestWhisper)
                    {
                        bestWhisper = judgement;
                    }

                    break;
                case RoleplayActs.Emoting:
                    if (judgement > bestEmote)
                    {
                        bestEmote = judgement;
                    }

                    break;
                case RoleplayActs.QuickEmoting:
                    if (judgement > bestQuickEmote)
                    {
                        bestQuickEmote = judgement;
                    }

                    break;
                case RoleplayActs.Subtling:
                    if (judgement > bestSubtle)
                    {
                        bestSubtle = judgement;
                    }

                    break;
                case RoleplayActs.Radio:
                    if (judgement > bestRadio)
                    {
                        bestRadio = judgement;
                    }

                    break;
                default:
                    Log.Warning($"Unknown roleplay action {action.Action} on entity {uid}!");
                    break;
            }
            action.Judgement = judgement; // set the judgement on the action
            var totaljudge = bestSay + bestWhisper + bestEmote + bestQuickEmote + bestSubtle + bestRadio;
            // how much of their current bankhole should we pay them?
            var payScalar = totaljudge * incentive.PaywardScalar;
            // now turn that payscale into something we can forcefeed the player account
            // get the bank account
            _bank.TryGetBalance(uid, out var bankBalance);
            var payAmount = (int) Math.Round(bankBalance * payScalar);
            // clamp the pay amount between 1 and 2000
            payAmount = Math.Clamp(payAmount, 1, 2000);
            // pay the player
            if (!_bank.TryBankDeposit(uid, payAmount))
            {
                Log.Warning($"Failed to deposit {payAmount} into bank account of entity {uid}!");
                return;
            }
            // tell the player they got paid!
            var message = Loc.GetString("coyote-rp-incentive-payward-message",
                ("amount", payAmount)
                );
            _popupSystem.PopupEntity(message, uid, uid, PopupType.LargeCaution);
        }
    }

    /// <summary>
    /// Passes judgement on the action
    /// Based on a set of criteria, it will return a judgement value
    /// It will be judged based on:
    /// - How long the text was
    /// - How many people were present
    /// - and thats it for now lol
    /// </summary>
    private void JudgeAction(RoleplayAction action, out float judgement)
    {
        // how long is a good action, for this?
        var goodlong = action.Action switch
        {
            RoleplayActs.Speaking => GoodlenSpeaking,
            RoleplayActs.Whispering => GoodlenWhispering,
            RoleplayActs.Emoting => GoodlenEmoting,
            RoleplayActs.QuickEmoting => GoodlenQuickEmoting,
            RoleplayActs.Subtling => GoodlenSubtling,
            RoleplayActs.Radio => GoodlenRadio,
            _ => 50,
        };
        // how many people were present for this action?
        var goodppl = action.Action switch
        {
            RoleplayActs.Speaking => GoodpplSpeaking,
            RoleplayActs.Whispering => GoodpplWhispering,
            RoleplayActs.Emoting => GoodpplEmoting,
            RoleplayActs.QuickEmoting => GoodpplQuickEmoting,
            RoleplayActs.Subtling => GoodpplSubtling,
            RoleplayActs.Radio => GoodpplRadio,
            _ => 1,
        };
        judgement = 1; // you get something for just showing up
        if (action.PeoplePresent >= goodppl)
        {
            // if there were enough people present, you get a bonus
            judgement += action.PeoplePresent;
        }
        if (action.Message != null && action.Message.Length >= goodlong)
        {
            // todo: add in some checks to catch if someones jsut doing AAAAAAAAAAAAAA
            // if the message was long enough, you get a bonus
            judgement += action.Message.Length / goodlong;
        }
        // good enough
    }
}
