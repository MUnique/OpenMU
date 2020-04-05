// <copyright file="OnlineCommand.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands
{
    using System;
    using System.Runtime.InteropServices;
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

        private const CharacterStatus MinimumStatus = CharacterStatus.Normal;

        /// <inheritdoc/>
        public CharacterStatus MinCharacterStatusRequirement => MinimumStatus;

        /// <inheritdoc />
        public string Key => Command;

        /// <inheritdoc />
        public void HandleCommand(Player player, string command)
        {
            try
            {
                if (player.GameContext is IGameServerContext gameServerContext)
                {
                    int maximumPlayers = gameServerContext.ServerConfiguration.MaximumPlayers;
                    int currentPlayers = (int)player.GameContext.PlayerList.Count;
                    player.ShowMessage($"[Online System] there's currently {currentPlayers}/{maximumPlayers} online players.");
                }
            }
            catch (ArgumentException e)
            {
                player.ShowMessage(e.Message);
            }
        }
    }
}