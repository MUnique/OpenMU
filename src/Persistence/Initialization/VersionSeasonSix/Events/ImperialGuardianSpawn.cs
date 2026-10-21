// <copyright file="ImperialGuardianSpawn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Events;

using MUnique.OpenMU.GameLogic.MiniGames.ImperialGuardian;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// A monster spawn of the imperial guardian event.
/// </summary>
/// <param name="Number">The number of the spawn within its map.</param>
/// <param name="MonsterNumber">The number of the monster.</param>
/// <param name="X">The x coordinate.</param>
/// <param name="Y">The y coordinate.</param>
/// <param name="Direction">The direction.</param>
/// <param name="Zone">The zone, starting at 0.</param>
/// <param name="Day">The day of the week.</param>
internal readonly record struct ImperialGuardianSpawn(short Number, short MonsterNumber, byte X, byte Y, Direction Direction, byte Zone, ImperialGuardianDay Day)
{
    /// <summary>
    /// Gets the wave number of the spawn, which is used by the event to spawn the monsters of a zone.
    /// </summary>
    public byte WaveNumber => ImperialGuardianEventDefinition.GetWaveNumber((byte)this.Day, this.Zone);
}
