// <copyright file="FieldMonsterSpawnCorrection.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

/// <summary>
/// The corrections of the automatic monster spawns of one field map, which the
/// <see cref="FieldMonsterSpawnsUpdatePlugInBase"/> applies to existing databases.
/// </summary>
/// <remarks>
/// The spawns are identified by their number, like in the map initializer, which defines the same spawns.
/// </remarks>
/// <param name="MapNumber">The number of the map.</param>
/// <param name="Spawns">The spawns which are added, or updated when a spawn with the same number exists already.</param>
/// <param name="RemovedSpawnNumbers">The numbers of the spawns which are removed.</param>
internal readonly record struct FieldMonsterSpawnCorrection(byte MapNumber, IReadOnlyList<FieldMonsterSpawn> Spawns, IReadOnlyList<short> RemovedSpawnNumbers);
