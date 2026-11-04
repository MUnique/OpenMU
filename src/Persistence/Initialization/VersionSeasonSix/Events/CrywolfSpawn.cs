// <copyright file="CrywolfSpawn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Events;

/// <summary>
/// A monster spawn of the army of Balgass on the crywolf map.
/// </summary>
/// <param name="Number">The number of the spawn.</param>
/// <param name="MonsterNumber">The number of the monster.</param>
/// <param name="X">The x coordinate.</param>
/// <param name="Y">The y coordinate.</param>
/// <param name="WaveNumber">The wave number, which is the number of the group.</param>
internal readonly record struct CrywolfSpawn(short Number, short MonsterNumber, byte X, byte Y, byte WaveNumber);
