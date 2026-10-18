// <copyright file="CrywolfSpawns.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Events;

/// <summary>
/// The monster spawns of the army of Balgass on the crywolf map, which are spawned by the crywolf event.
/// </summary>
/// <remarks>
/// The wave number is the number of the group of the original game. The group 5 is Balgass,
/// the first spawn of each other group is its leader, the Dark Elf.
/// </remarks>
internal static class CrywolfSpawns
{
    /// <summary>
    /// Gets the spawns.
    /// </summary>
    public static IReadOnlyList<CrywolfSpawn> Spawns { get; } =
    [
        new(1020, 340, 110, 77, 1),
        new(1021, 341, 108, 73, 1),
        new(1022, 341, 110, 73, 1),
        new(1023, 341, 112, 73, 1),
        new(1024, 341, 109, 75, 1),
        new(1025, 341, 111, 75, 1),
        new(1040, 340, 125, 77, 2),
        new(1041, 341, 125, 73, 2),
        new(1042, 341, 123, 73, 2),
        new(1043, 341, 127, 73, 2),
        new(1044, 341, 124, 75, 2),
        new(1045, 341, 126, 75, 2),
        new(1060, 340, 119, 83, 3),
        new(1061, 341, 119, 77, 3),
        new(1062, 341, 117, 77, 3),
        new(1063, 341, 121, 77, 3),
        new(1064, 344, 118, 79, 3),
        new(1065, 344, 120, 79, 3),
        new(1066, 345, 116, 81, 3),
        new(1067, 345, 119, 81, 3),
        new(1068, 345, 122, 81, 3),
        new(1080, 340, 119, 87, 4),
        new(1081, 344, 119, 90, 4),
        new(1082, 348, 122, 90, 4),
        new(1083, 348, 116, 90, 4),
        new(1100, 349, 110, 79, 5),
        new(1120, 340, 77, 28, 6),
        new(1121, 341, 81, 25, 6),
        new(1122, 341, 81, 27, 6),
        new(1123, 341, 81, 29, 6),
        new(1124, 341, 79, 26, 6),
        new(1125, 341, 79, 28, 6),
        new(1140, 340, 77, 42, 7),
        new(1141, 341, 79, 38, 7),
        new(1142, 341, 81, 37, 7),
        new(1143, 341, 81, 39, 7),
        new(1144, 341, 81, 41, 7),
        new(1145, 341, 79, 40, 7),
        new(1160, 340, 70, 34, 8),
        new(1161, 341, 77, 31, 8),
        new(1162, 341, 77, 33, 8),
        new(1163, 341, 77, 35, 8),
        new(1164, 344, 75, 32, 8),
        new(1165, 344, 75, 34, 8),
        new(1166, 345, 73, 33, 8),
        new(1167, 345, 73, 29, 8),
        new(1168, 345, 73, 37, 8),
        new(1180, 340, 65, 34, 9),
        new(1181, 344, 62, 34, 9),
        new(1182, 348, 62, 37, 9),
        new(1183, 348, 62, 31, 9),
        new(1200, 340, 167, 27, 10),
        new(1201, 341, 163, 29, 10),
        new(1202, 341, 163, 27, 10),
        new(1203, 341, 163, 25, 10),
        new(1204, 341, 165, 26, 10),
        new(1205, 341, 165, 28, 10),
        new(1220, 340, 167, 39, 11),
        new(1221, 341, 165, 38, 11),
        new(1222, 341, 163, 37, 11),
        new(1223, 341, 163, 39, 11),
        new(1224, 341, 165, 40, 11),
        new(1225, 341, 163, 41, 11),
        new(1240, 340, 174, 33, 12),
        new(1241, 341, 167, 35, 12),
        new(1242, 341, 167, 33, 12),
        new(1243, 341, 167, 31, 12),
        new(1244, 344, 169, 32, 12),
        new(1245, 344, 169, 34, 12),
        new(1246, 345, 171, 33, 12),
        new(1247, 345, 171, 30, 12),
        new(1248, 345, 171, 36, 12),
        new(1260, 340, 179, 33, 13),
        new(1261, 344, 183, 33, 13),
        new(1262, 348, 183, 36, 13),
        new(1263, 348, 183, 30, 13),
    ];
}
