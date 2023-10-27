// <copyright file="MonsterSpawnArea.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;

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

    /// <summary>
    /// The monster spawns automatically during a wave of an event.
    /// </summary>
    /// <remarks>
    /// For example, at devil square different monsters spawn in different waves.
    /// </remarks>
    AutomaticDuringWave,

    /// <summary>
    /// The monster spawns once at the start of a wave of an event.
    /// </summary>
    /// <remarks>
    /// For example, at devil square there is a wave of bosses, which spawn only once.
    /// </remarks>
    OnceAtWaveStart,

    /// <summary>
    /// The monster spawns manually controlled by the code of an event.
    /// </summary>
    /// <remarks>
    /// For example chaos castle enemies, because their number is not known beforehand.
    /// </remarks>
    ManuallyForEvent,

    /// <summary>
    /// The object is wandering between maps. It spawns just at one spawn area
    /// at the same time.
    /// </summary>
    /// <remarks>
    /// Used for wandering merchants.
    /// </remarks>
    Wandering,
}

/// <summary>
/// Defines an monster spawn area.
/// </summary>
[Cloneable]
public partial class MonsterSpawnArea
{
    /// <summary>
    /// Gets or sets the monster definition.
    /// </summary>
    public virtual MonsterDefinition? MonsterDefinition { get; set; }

    /// <summary>
    /// Gets or sets the game map.
    /// </summary>
    public virtual GameMapDefinition? GameMap { get; set; }

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

    /// <summary>
    /// Gets or sets the wave to which this spawn area belongs to.
    /// </summary>
    public byte WaveNumber { get; set; }

    /// <summary>
    /// Gets or sets the maximum health (override) just for this spawn area.
    /// If <c>null</c>, the default health of the <see cref="MonsterDefinition"/> applies.
    /// </summary>
    public int? MaximumHealthOverride { get; set; }

    /// <inheritdoc/>
    public override string ToString()
    {
        var isPoint = this.IsPoint();
        var result = isPoint
            ? $"{this.MonsterDefinition?.Designation} - Quantity: {this.Quantity} - At: {this.X1}/{this.Y1}"
            : $"{this.MonsterDefinition?.Designation} - Quantity: {this.Quantity} - Area: {this.X1}/{this.Y1} to {this.X2}/{this.Y2}";

        if (this.SpawnTrigger == SpawnTrigger.AutomaticDuringWave || this.SpawnTrigger == SpawnTrigger.OnceAtWaveStart)
        {
            result += $" - Wave: {this.WaveNumber}";
        }

        return result;
    }

    /// <summary>
    /// Determines whether this instance is a spawn point.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if this instance is point; otherwise, <c>false</c>.
    /// </returns>
    public bool IsPoint() => this.X1 == this.X2 && this.Y1 == this.Y2;
}