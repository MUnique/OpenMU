// <copyright file="GameMapDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration
{
    using System.Collections.Generic;

    /// <summary>
    /// Configuration of a map. Contains all information to create an instance of a GameMap.
    /// </summary>
    /// <remarks>
    /// Some maps have different possible status, for expample the crywolf map:
    ///     1) Balgass is undeafeated - monsters are not dropping special items
    ///     2) Balgass is defeated - monsters are dropping special items
    ///     3) Crywolf event is ongoing - The normal monsters are not there(?), but event monsters.
    /// For each of this status, there exist different terrain maps (safezones are different, etc.).
    /// To reflect this requirement on this data model, for each status there must be one game map definition.
    /// The switch between this status and its corresponding game map definitions should be done in game logic.
    /// </remarks>
    public class GameMapDefinition
    {
        /// <summary>
        /// Gets or sets the number of the map.
        /// </summary>
        /// <remarks>
        /// This number is identifying the map on the client.
        /// </remarks>
        public short Number { get; set; }

        /// <summary>
        /// Gets or sets the name of the map.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the terrain data.
        /// </summary>
        /// <remarks>
        /// Content of the *.att file in the original server.
        /// </remarks>
        public byte[] TerrainData { get; set; }

        /// <summary>
        /// Gets or sets the defined monster spawn areas.
        /// </summary>
        public virtual ICollection<MonsterSpawnArea> MonsterSpawns { get; protected set; }

        /// <summary>
        /// Gets or sets the enter gates, though which the player can move to other maps.
        /// </summary>
        public virtual ICollection<EnterGate> Gates { get; protected set; }

        /// <summary>
        /// Gets or sets the exp multiplier for this map.
        /// </summary>
        /// <value>
        /// The exp multiplier.
        /// </value>
        public double ExpMultiplier { get; set; }

        /// <summary>
        /// Gets or sets the gate to which the player will be brought when he died. Is contained in <see cref="SpawnGates"/>.
        /// </summary>
        /// <remarks>
        /// NO-DO: Instead of this property, add a flag to ExitGate. Problem: Safezone of a map can be in another map!
        /// Examples: Player dies in the Dungeon map, respawns in Lorencia. Or Player dies in Kalima 1-6, respawns in Devias.
        /// </remarks>
        public virtual ExitGate DeathSafezone { get; set; }

        /// <summary>
        /// Gets or sets the spawn gates.
        /// </summary>
        public virtual ICollection<ExitGate> SpawnGates { get; protected set; }

        /// <summary>
        /// Gets or sets the DropItemGroup of this map.
        /// Some maps contain different drops. Examples: land of trials drops ancient items, kanturu drops gemstones.
        /// </summary>
        public virtual ICollection<DropItemGroup> DropItemGroups { get; protected set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("{0} - {1}", this.Number, this.Name);
        }
    }
}
