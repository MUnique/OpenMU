﻿// <copyright file="MoveCommand.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands
{
    using System.Linq;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.PlayerActions;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A chat command plugin which handles move commands.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.IChatCommandPlugIn" />
    [Guid("4564AE2B-4819-4155-B5B2-FE2ED0CF7A7F")]
    [PlugIn("Move chat command", "Handles the chat command '/move map'. Move the character to the specified map.")]
    public class MoveCommand : IChatCommandPlugIn
    {
        private const string CommandKey = "/move";

        private readonly WarpAction warpAction = new WarpAction();

        /// <inheritdoc />
        public string Key => CommandKey;

        /// <inheritdoc />
        public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.Normal;

        /// <inheritdoc />
        public void HandleCommand(Player player, string command)
        {
            var map = command.Split(' ').Skip(1).FirstOrDefault();
            if (map == null)
            {
                return;
            }

            var warpInfo = GetWarpInfo(map, player);
            if (warpInfo == null)
            {
                return;
            }

            this.warpAction.WarpTo(player, warpInfo);
        }

        private static WarpInfo GetWarpInfo(string mapName, Player player)
        {
            return player.GameContext.Configuration.WarpList?.FirstOrDefault(info => info.Name.ToLower().Equals(mapName.ToLower()));
        }
    }
}
