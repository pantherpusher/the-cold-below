using Content.Server.Administration;
using Content.Server.Chat.Managers;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server.Chat.Commands
{
    [AdminCommand(AdminFlags.Adminchat)]
    internal sealed class AdminAhelpCommand : IConsoleCommand
    {
        public string Command => "ahelpecho";
        public string Description => "Hacky passthrough for ahelps to print to chat properly.";
        public string Help => "ahelpecho <text>";

        public void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            var player = shell.Player;

            if (player == null)
            {
                shell.WriteError("You can't run this command locally.");
                return;
            }

            if (args.Length < 1)
                return;

            var message = string.Join(" ", args).Trim();
            if (string.IsNullOrEmpty(message))
                return;

            IoCManager.Resolve<IChatManager>().TrySendOOCMessage(player, message, OOCChatType.AdminHelp);
        }
    }
}
