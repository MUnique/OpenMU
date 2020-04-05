// <copyright file="OnlineCommand.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The online command which shows the count of connected players.
    /// </summary>
    [Guid("94A93A8B-75C4-473F-8CE2-74943668D251")]
    [PlugIn("Online Command", "Shows online players count.")]
    [ChatCommandHelp(Command, null, MinimumStatus)]
    public class OnlineCommand : IChatCommandPlugIn
    {
        private const string Command = "/online";

        private const CharacterStatus MinimumStatus = CharacterStatus.GameMaster;

        /// <inheritdoc/>
        public CharacterStatus MinCharacterStatusRequirement => MinimumStatus;

        /// <inheritdoc />
        public string Key => Command;

        /// <inheritdoc />
        public void HandleCommand(Player player, string command)
        {
            try
            {
                int maximumPlayers = 1000; // need to get from gameserver configirution.
                player.ShowMessage($"[Online System] there's currently {(int)player.GameContext.PlayerList.Count}/{maximumPlayers} online players.");
            }
            catch (ArgumentException e)
            {
                player.ShowMessage(e.Message);
            }
        }

    }
}