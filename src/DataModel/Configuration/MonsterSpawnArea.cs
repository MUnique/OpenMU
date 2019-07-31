// <copyright file="MonsterSpawnArea.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration
{
    /// <summary>
    /// Defines the trigger when a monster spawns.
    /// </summary>
    public enum SpawnTrigger
    {
        /// <summary>
        /// The monster spawns and respawns automatically.
        /// </summary>
        Automatic,

        /// <summary>
        /// The monster spawns automatically during an event.
        /// </summary>
        AutomaticDuringEvent,

        /// <summary>
        /// The monster spawns just once at the beginning of an event.
        /// </summary>
        /// <remarks>
        /// For example blood castle gates, statues. Also golden monsters.
        /// </remarks>
        OnceAtEventStart,
    }

    /// <summary>
    /// Defines an monster spawn area.
    /// </summary>
    public class MonsterSpawnArea
    {
        /// <summary>
        /// Gets or sets the monster definition.
        /// </summary>
        public virtual MonsterDefinition MonsterDefinition { get; set; }

        /// <summary>
        /// Gets or sets the game map.
        /// </summary>
        public virtual GameMapDefinition GameMap { get; set; }

        /// <summary>
        /// Gets or sets the upper left corner x coordinate.
        /// </summary>
        public byte X1 { get; set; }

        /// <summary>
        /// Gets or sets the upper left corner y coordinate.
        /// </summary>
        public byte Y1 { get; set; }

        /// <summary>
        /// Gets or sets the bottom right corner x coordinate.
        /// </summary>
        public byte X2 { get; set; }

        /// <summary>
        /// Gets or sets the bottom right corner y coordinate.
        /// </summary>
        public byte Y2 { get; set; }

        /// <summary>
        /// Gets or sets the looking direction when spawning.
        /// </summary>
        public Direction Direction { get; set; }

        /// <summary>
        /// Gets or sets the quantity of monsters which should spawn in the defined area.
        /// </summary>
        public short Quantity { get; set; }

        /// <summary>
        /// Gets or sets the spawn trigger.
        /// </summary>
        public SpawnTrigger SpawnTrigger { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (this.X1 != this.X2 || this.Y1 != this.Y2)
            {
                return $"{this.MonsterDefinition.Designation} - Quantity: {this.Quantity} - Area: {this.X1}/{this.Y1} - {this.X2}/{this.Y2}";
            }
            else
            {
                return $"{this.MonsterDefinition.Designation} - Quantity: {this.Quantity} - At: {this.X1}/{this.Y1}";
            }
        }
    }
}
