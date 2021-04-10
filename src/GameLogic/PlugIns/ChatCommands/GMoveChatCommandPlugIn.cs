// <copyright file="GMoveChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.GameMaster
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
    using MUnique.OpenMU.Pathfinding;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A chat command plugin which handles gm move commands.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.IChatCommandPlugIn" />
    [Guid("9163C3EA-6722-4E55-A109-20C163C05266")]
    [PlugIn("GMove chat command", "Handles the chat command '/gmove <characterName> <map> <x?> <y?>'. Move a character to a specified map and coordinates.")]
    [ChatCommandHelp(Command, typeof(GMoveChatCommandArgs), CharacterStatus.GameMaster)]
    public class GMoveChatCommandPlugIn : ChatCommandPlugInBase<GMoveChatCommandArgs>
    {
        private const string Command = "/gmove";

        /// <inheritdoc />
        public override string Key => Command;

        /// <inheritdoc/>
        public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

        /// <inheritdoc />
        protected override void DoHandleCommand(Player gameMaster, GMoveChatCommandArgs arguments)
        {
            var player = this.GetPlayerByCharacterName(gameMaster, arguments.CharacterName!);
            var gate = this.GetWarpDestination(gameMaster, arguments.Map!, arguments.Coordinates);
            player.WarpTo(gate);

            if (player.Name.Equals(gameMaster.Name))
            {
                this.ShowMessageTo(gameMaster, $"[{this.Key}] You have been moved to {gate.Map!.Name} at {player.Position.X}, {player.Position.Y}");
            }
            else
            {
                this.ShowMessageTo(player, "You have been moved by the game master.");
                this.ShowMessageTo(gameMaster, $"[{this.Key}] {arguments.CharacterName} has been moved to {gate.Map!.Name} at {player.Position.X}, {player.Position.Y}");
            }
        }

        /// <summary>
        /// Gets a warp destination by map id/name and coordinates.
        /// </summary>
        /// <param name="gameMaster">The game master.</param>
        /// <param name="map">The name or id of the map.</param>
        /// <param name="coordinates">The coordinates X and Y.</param>
        /// <returns>An instance of the ExitGate.</returns>
        private ExitGate GetWarpDestination(Player gameMaster, string map, Point coordinates)
        {
            GameMapDefinition mapDefinition = ushort.TryParse(map, out var mapId)
                ? this.GetMapDefinationByMapId(gameMaster, mapId)
                : this.GetMapDefinationByMapName(gameMaster, map);

            return this.GetGateByCoordinates(mapDefinition, map, coordinates);
        }

        /// <summary>
        /// Gets a map definition by map id.
        /// </summary>
        /// <param name="gameMaster">The game master.</param>
        /// <param name="mapId">The map id.</param>
        /// <returns>An instance of the GameMapDefinition.</returns>
        private GameMapDefinition GetMapDefinationByMapId(Player gameMaster, ushort mapId)
        {
            return gameMaster.GameContext.GetMap(mapId)?.Definition
                ?? throw new ArgumentException($"Map {mapId} not found.");
        }

        /// <summary>
        /// Gets a map definition by map name.
        /// </summary>
        /// <param name="gameMaster">The game master.</param>
        /// <param name="mapName">The name of the map.</param>
        /// <returns>An instance of the GameMapDefinition.</returns>
        private GameMapDefinition GetMapDefinationByMapName(Player gameMaster, string mapName)
        {
            if (string.IsNullOrWhiteSpace(mapName))
            {
                throw new ArgumentException($"Map is required.");
            }

            return gameMaster.GameContext.Configuration.Maps.FirstOrDefault(x => x.Name.Equals(mapName, StringComparison.OrdinalIgnoreCase))
                ?? throw new ArgumentException($"Map {mapName} not found.");
        }

        /// <summary>
        /// Gets a gate by map definition and coordinates.
        /// </summary>
        /// <param name="mapDefinition">The map defination.</param>
        /// <param name="map">The name or id of the map.</param>
        /// <param name="coordinates">The coordinates X and Y.</param>
        /// <returns>An instance of the ExitGate.</returns>
        private ExitGate GetGateByCoordinates(GameMapDefinition mapDefinition, string map, Point coordinates)
        {
            if (coordinates.X == default && coordinates.Y == default)
            {
                return mapDefinition.SafezoneMap?.ExitGates?.FirstOrDefault()
                    ?? throw new ArgumentException($"Map {map} does not have a safe zone, please enter the coordinates.");
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
    }
}
