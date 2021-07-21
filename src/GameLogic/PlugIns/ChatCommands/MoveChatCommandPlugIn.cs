// <copyright file="MoveChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.PlayerActions;
    using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A chat command plugin which handles move commands.
    /// </summary>
    [Guid("4564AE2B-4819-4155-B5B2-FE2ED0CF7A7F")]
    [PlugIn("Move chat command", "Handles the chat command '/move <target> <mapIdOrName?> <x?> <y?>'. Move the character to the specified destination.")]
    [ChatCommandHelp(Command, typeof(MoveChatCommandArgs), CharacterStatus.Normal)]
    public class MoveChatCommandPlugIn : ChatCommandPlugInBase<MoveChatCommandArgs>
    {
        private const string Command = "/move";

        /// <inheritdoc />
        public override string Key => Command;

        /// <inheritdoc/>
        public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.Normal;

        /// <inheritdoc />
        protected override void DoHandleCommand(Player sender, MoveChatCommandArgs arguments)
        {
            var senderIsGameMaster = sender.SelectedCharacter?.CharacterStatus == CharacterStatus.GameMaster;
            var isGameMasterWarpingCharacter = senderIsGameMaster && !string.IsNullOrWhiteSpace(arguments.MapIdOrName);

            if (isGameMasterWarpingCharacter)
            {
                var targetPlayer = this.GetPlayerByCharacterName(sender, arguments.Target!);
                var exitGate = this.GetExitGate(sender, arguments.MapIdOrName!, arguments.Coordinates);
                targetPlayer.WarpTo(exitGate);

                if (!targetPlayer.Name.Equals(sender.Name))
                {
                    this.ShowMessageTo(targetPlayer, "You have been moved by the game master.");
                    this.ShowMessageTo(sender, $"[{this.Key}] {targetPlayer.Name} has been moved to {exitGate!.Map!.Name} at {targetPlayer.Position.X}, {targetPlayer.Position.Y}");
                }
            }
            else
            {
                var warpInfo = this.GetWarpInfo(sender, arguments.Target!);
                if (warpInfo != null)
                {
                    new WarpAction().WarpTo(sender, warpInfo);
                }
            }
        }
    }
}
