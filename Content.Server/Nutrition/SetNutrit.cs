using Content.Server.Administration;
using Content.Server.Database.Migrations.Postgres;
using Content.Shared.Administration;
using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using Robust.Shared.Console;
using System.Linq;
using Content.Shared._Coyote.Needs;

namespace Content.Server.Nutrition;

[AdminCommand(AdminFlags.Debug)]
public sealed class SetNutrit : LocalizedEntityCommands
{
    public override string Command => "setnutrit";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        var player = shell.Player;
        if (player == null)
        {
            shell.WriteError(Loc.GetString("cmd-nutrition-error-player"));
            return;
        }

        if (player.AttachedEntity is not { Valid: true } playerEntity)
        {
            shell.WriteError(Loc.GetString("cmd-nutrition-error-entity"));
            return;
        }

        if (args.Length != 2)
        {
            shell.WriteError(
                Loc.GetString(
                    "shell-wrong-arguments-number-need-specific",
                    ("properAmount", 2),
                    ("currentAmount", args.Length)));
            return;
        }
        if (!EntityManager.TryGetComponent(playerEntity, out NeedsComponent? needy))
        {
            shell.WriteError(Loc.GetString("cmd-nutrition-error-component", ("comp", nameof(NeedsComponent))));
            return;
        }
        var needsSystem = EntityManager.System<SharedNeedsSystem>();

        var systemString = args[0];
        switch (systemString)
        {
            case "hunger":
            {
                if (!needsSystem.UsesHunger(playerEntity, needy))
                {
                    shell.WriteError("They dont use hunger");
                    return;
                }
                if (!Enum.TryParse(args[1], out NeedThreshold needThreshold))
                {
                    shell.WriteError(Loc.GetString("cmd-setnutrit-error-invalid-threshold",
                        ("thresholdType", nameof(NeedThreshold)),
                        ("thresholdString", args[1])
                    ));
                    return;
                }

                needsSystem.SetHungerToThreshold(
                    playerEntity,
                    needThreshold,
                    needy);
                return;
            }
            case "thirst":
            {
                if (!needsSystem.UsesThirst(playerEntity, needy))
                {
                    shell.WriteError("They dont use thirst");
                    return;
                }

                if (!Enum.TryParse(args[1], out NeedThreshold thirstThreshold))
                {
                    shell.WriteError(Loc.GetString("cmd-setnutrit-error-invalid-threshold",
                         ("thresholdType", nameof(NeedThreshold)),
                         ("thresholdString", args[1])
                     ));
                    return;
                }

                needsSystem.SetThirstToThreshold(
                    playerEntity,
                    thirstThreshold,
                    needy);
                return;
            }
            default:
            {
                shell.WriteError($"invalid nutrition system ${systemString}");
                return;
            }
        }
    }

    public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        switch (args.Length)
        {
            case 1:
            {
                string[] kinds = { "hunger", "thirst" };
                return CompletionResult.FromHintOptions(kinds, "nutrition system");
            }
            case 2:
            {
                return args[0] switch
                {
                    "hunger" => CompletionResult.FromHintOptions(Enum.GetNames<NeedThreshold>(), nameof(NeedThreshold)),
                    "thirst" => CompletionResult.FromHintOptions(Enum.GetNames<NeedThreshold>(), nameof(NeedThreshold)),
                    _ => CompletionResult.Empty,
                };
            }
            default:
                return CompletionResult.Empty;
        }
    }
}
