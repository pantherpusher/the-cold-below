using Content.Server.Administration;
using Content.Shared._Coyote.Needs;
using Content.Shared.Administration;
using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using Robust.Shared.Console;

namespace Content.Server.Nutrition;

[AdminCommand(AdminFlags.Debug)]
public sealed class Hungry : LocalizedEntityCommands
{
    public override string Command => "hungry";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        var player = shell.Player;
        if (player == null)
        {
            shell.WriteError(Loc.GetString("cmd-nutrition-error-player"));
            return;
        }

        if (player.AttachedEntity is not {Valid: true} playerEntity)
        {
            shell.WriteError(
                Loc.GetString(
                    "cmd-nutrition-error-entity"));
            return;
        }

        if (!EntityManager.TryGetComponent(playerEntity, out NeedsComponent? needy))
        {
            shell.WriteError(
                Loc.GetString(
                    "cmd-nutrition-error-component",
                    ("comp", nameof(NeedsComponent))));
            return;
        }

        var lowestHunger = EntityManager.System<SharedNeedsSystem>()
            .GetHungerMinThreshold(
                playerEntity,
                NeedThreshold.Critical,
                needy);
        if (lowestHunger == null)
        {
            shell.WriteError("BAD");
            return;
        }
        EntityManager.System<SharedNeedsSystem>()
            .SetHunger(
                playerEntity,
                lowestHunger.Value,
                needy);
    }
}
