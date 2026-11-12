// <copyright file="FieldMonsterSpawn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

/// <summary>
/// An automatic monster spawn on a field map, which is shared between the map initializer and
/// the update plugin which brings the same spawns to existing databases.
/// </summary>
/// <remarks>
/// When the coordinates describe a single tile, the monster always spawns at this spot;
/// otherwise it spawns at a random tile of the area.
/// </remarks>
/// <param name="Number">The number of the spawn, which makes up its persisted identifier together with the map number.</param>
/// <param name="MonsterNumber">The number of the monster.</param>
/// <param name="X1">The first x coordinate of the area.</param>
/// <param name="X2">The second x coordinate of the area.</param>
/// <param name="Y1">The first y coordinate of the area.</param>
/// <param name="Y2">The second y coordinate of the area.</param>
/// <param name="Quantity">The number of monsters which are spawned in the area.</param>
internal readonly record struct FieldMonsterSpawn(short Number, short MonsterNumber, byte X1, byte X2, byte Y1, byte Y2, short Quantity);
