// <copyright file="NpcSpawnCorrection.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// The values of one NPC spawn, which an NPC spawn update (like <see cref="NpcSpawnsUpdatePlugInSeason6"/>) applies to existing databases.
/// </summary>
/// <param name="MapNumber">The number of the map.</param>
/// <param name="SpawnNumber">The number of the spawn, like in the map initializer.</param>
/// <param name="NpcNumber">The number of the NPC of the spawn.</param>
/// <param name="X">The x coordinate.</param>
/// <param name="Y">The y coordinate.</param>
/// <param name="Direction">The direction into which the NPC looks.</param>
/// <param name="IsAdded">If set to <c>true</c>, the spawn is new in the data of the game version and created when it's missing.</param>
internal readonly record struct NpcSpawnCorrection(byte MapNumber, short SpawnNumber, short NpcNumber, byte X, byte Y, Direction Direction, bool IsAdded = false);
