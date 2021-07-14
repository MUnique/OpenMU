// <copyright file="ChatCommandPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands
{
    using System;
    using System.Linq;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Pathfinding;

    /// <summary>
    /// The base of every chat command plug in.
    /// </summary>
    /// <typeparam name="T">The type of arguments base.</typeparam>
    public abstract class ChatCommandPlugInBase<T> : IChatCommandPlugIn
        where T : ArgumentsBase, new()
    {
        /// <inheritdoc/>
        public abstract string Key { get; }

        /// <inheritdoc/>
        public abstract CharacterStatus MinCharacterStatusRequirement { get; }

        /// <inheritdoc/>
        public virtual void HandleCommand(Player player, string command)
        {
            try
            {
                var arguments = command.ParseArguments<T>();
                this.DoHandleCommand(player, arguments);
            }
            catch (ArgumentException argEx)
            {
                this.ShowMessageTo(player, $"[{this.Key}] {argEx.Message}");
            }
            catch (Exception ex)
            {
                player.Logger.LogError(ex, $"Unexpected error handling the chat command '{this.Key}'.", command);
            }
        }

        /// <summary>
        /// Handles the chat command safely.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="arguments">The arguments.</param>
        protected abstract void DoHandleCommand(Player player, T arguments);

        /// <summary>
        /// Shows a message to a player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="message">The message.</param>
        /// <param name="messageType">The message type.</param>
        protected void ShowMessageTo(Player player, string message, MessageType messageType = MessageType.BlueNormal)
        {
            player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage(message, messageType);
        }

        /// <summary>
        /// Gets a player by his character name.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="characterName">The character name.</param>
        /// <returns>The target player.</returns>
        protected Player GetPlayerByCharacterName(Player player, string characterName)
        {
            if (string.IsNullOrWhiteSpace(characterName))
            {
                throw new ArgumentException("Character name is required.");
            }

            return player.GameContext.GetPlayerByCharacterName(characterName)
                ?? throw new ArgumentException($"Character {characterName} not found.");
        }

        /// <summary>
        /// Gets a guild id by name.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="guildName">The guild name.</param>
        /// <returns>The guild id.</returns>
        protected uint GetGuildIdByName(Player player, string guildName)
        {
            var guildServer = (player.GameContext as IGameServerContext)!.GuildServer;
            var guildId = guildServer.GetGuildIdByName(guildName);

            if (guildId == default(uint))
            {
                throw new ArgumentException($"Guild {guildName} not found.");
            }

            return guildId;
        }

        /// <summary>
        /// Gets a exit gate.
        /// </summary>
        /// <param name="gameMaster">The game master.</param>
        /// <param name="map">The name or id of the map.</param>
        /// <param name="coordinates">The coordinates X and Y.</param>
        /// <returns>The ExitGate.</returns>
        protected ExitGate GetExitGate(Player gameMaster, string map, Point coordinates)
        {
            if (coordinates.X == default && coordinates.Y == default)
            {
                return this.GetWarpInfo(gameMaster, map)?.Gate
                    ?? throw new ArgumentException($"Map {map} not found.");
            }

            var mapDefinition = ushort.TryParse(map, out var mapId)
                ? gameMaster.GameContext.GetMap(mapId)?.Definition
                : gameMaster.GameContext.Configuration.Maps.FirstOrDefault(x => x.Name.Equals(map, StringComparison.OrdinalIgnoreCase));

            if (mapDefinition == null)
            {
                throw new ArgumentException($"Map {map} not found.");
            }

            return new ExitGate
            {
                Map = mapDefinition,
                X1 = coordinates.X,
                X2 = coordinates.X,
                Y1 = coordinates.Y,
                Y2 = coordinates.Y,
            };
        }

        /// <summary>
        /// Gets a warp info.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="map">The name or id of the map.</param>
        /// <returns>The WarpInfo.</returns>
        protected WarpInfo? GetWarpInfo(Player player, string map)
        {
            var warpList = player.GameContext.Configuration.WarpList;
            return ushort.TryParse(map, out var mapId)
                ? warpList.FirstOrDefault(info => info.Gate?.Map?.Number == mapId)
                : warpList.FirstOrDefault(info => info.Name.Equals(map, StringComparison.OrdinalIgnoreCase));
        }
    }
}
