// <copyright file="CrywolfTerrain.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Crywolf;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// The changes of the terrain of the crywolf map, while the fortress isn't in peace.
/// </summary>
/// <remarks>
/// The game client loads a terrain file for each occupation state by itself. The terrain files of
/// the war and the occupation only differ from the one of the peace in these areas: the safezone
/// of the fortress is removed, and some areas are blocked. They were taken from the terrain files.
/// </remarks>
internal static class CrywolfTerrain
{
    /// <summary>
    /// Gets the areas, which aren't a safezone while the fortress isn't in peace.
    /// </summary>
    public static IReadOnlyList<(byte X1, byte Y1, byte X2, byte Y2)> SafezoneRemovedAreas { get; } =
    [
        new(92, 0, 157, 5),
        new(92, 6, 156, 13),
        new(91, 11, 91, 43),
        new(92, 14, 155, 15),
        new(90, 16, 90, 39),
        new(92, 16, 92, 46),
        new(94, 16, 155, 28),
        new(93, 18, 93, 48),
        new(86, 28, 89, 30),
        new(94, 29, 153, 45),
        new(89, 31, 89, 38),
        new(87, 35, 88, 38),
        new(154, 35, 155, 36),
        new(154, 37, 154, 44),
        new(94, 46, 152, 46),
        new(94, 47, 151, 47),
        new(94, 48, 150, 48),
        new(94, 49, 149, 49),
        new(95, 50, 149, 50),
        new(96, 51, 147, 52),
        new(96, 53, 146, 53),
        new(97, 54, 145, 54),
        new(98, 55, 144, 55),
        new(99, 56, 143, 56),
        new(101, 57, 141, 57),
        new(102, 58, 140, 58),
        new(104, 59, 139, 59),
        new(107, 60, 137, 60),
        new(110, 61, 136, 61),
        new(114, 62, 135, 62),
        new(115, 63, 125, 64),
        new(116, 65, 117, 65),
        new(124, 65, 125, 65),
        new(101, 252, 110, 253),
        new(96, 253, 100, 253),
        new(111, 253, 111, 253),
        new(141, 253, 151, 253),
    ];

    /// <summary>
    /// Gets the areas, which are blocked while the fortress isn't in peace.
    /// </summary>
    public static IReadOnlyList<(byte X1, byte Y1, byte X2, byte Y2)> BlockedAreas { get; } =
    [
        new(92, 0, 157, 5),
        new(92, 6, 101, 6),
        new(108, 6, 132, 6),
        new(146, 6, 156, 6),
        new(92, 7, 99, 8),
        new(110, 7, 130, 7),
        new(148, 7, 156, 7),
        new(112, 8, 129, 8),
        new(150, 8, 156, 9),
        new(92, 9, 98, 9),
        new(113, 9, 119, 9),
        new(122, 9, 127, 10),
        new(92, 10, 97, 15),
        new(114, 10, 119, 10),
        new(151, 10, 156, 13),
        new(115, 11, 117, 12),
        new(124, 11, 126, 11),
        new(91, 12, 91, 28),
        new(124, 12, 125, 12),
        new(148, 12, 150, 29),
        new(103, 13, 105, 14),
        new(115, 13, 116, 13),
        new(125, 13, 125, 13),
        new(135, 13, 141, 14),
        new(147, 13, 147, 21),
        new(146, 14, 146, 20),
        new(151, 14, 155, 28),
        new(135, 15, 140, 15),
        new(144, 15, 145, 19),
        new(90, 16, 90, 28),
        new(92, 16, 92, 28),
        new(94, 16, 97, 22),
        new(143, 16, 143, 18),
        new(123, 17, 123, 24),
        new(142, 17, 142, 18),
        new(93, 18, 93, 28),
        new(118, 18, 118, 24),
        new(145, 20, 145, 20),
        new(124, 21, 124, 24),
        new(115, 22, 117, 23),
        new(125, 22, 127, 23),
        new(94, 23, 95, 28),
        new(111, 24, 112, 29),
        new(116, 24, 117, 24),
        new(125, 24, 126, 24),
        new(145, 24, 147, 29),
        new(113, 25, 113, 29),
        new(117, 25, 117, 25),
        new(129, 25, 130, 29),
        new(114, 26, 114, 28),
        new(128, 26, 128, 28),
        new(96, 27, 100, 28),
        new(131, 27, 131, 29),
        new(142, 27, 144, 28),
        new(87, 28, 89, 28),
        new(107, 28, 110, 29),
        new(119, 28, 122, 35),
        new(106, 29, 106, 29),
        new(118, 29, 118, 34),
        new(123, 29, 123, 34),
        new(132, 29, 135, 29),
        new(151, 29, 153, 29),
        new(117, 30, 117, 33),
        new(124, 30, 124, 33),
        new(106, 34, 112, 34),
        new(129, 34, 135, 34),
        new(145, 34, 153, 45),
        new(87, 35, 100, 36),
        new(110, 35, 113, 35),
        new(128, 35, 132, 35),
        new(143, 35, 144, 36),
        new(154, 35, 155, 36),
        new(111, 36, 113, 37),
        new(128, 36, 130, 36),
        new(91, 37, 98, 37),
        new(129, 37, 130, 37),
        new(144, 37, 144, 55),
        new(154, 37, 154, 43),
        new(91, 38, 97, 43),
        new(111, 38, 112, 38),
        new(130, 38, 130, 38),
        new(115, 39, 118, 41),
        new(124, 39, 126, 41),
        new(98, 40, 98, 54),
        new(114, 40, 114, 41),
        new(123, 40, 123, 45),
        new(127, 40, 127, 40),
        new(99, 41, 99, 55),
        new(100, 42, 100, 56),
        new(116, 42, 118, 42),
        new(124, 42, 125, 42),
        new(101, 43, 101, 45),
        new(118, 43, 118, 45),
        new(124, 43, 124, 45),
        new(92, 44, 97, 46),
        new(142, 45, 143, 55),
        new(199, 45, 199, 45),
        new(131, 46, 132, 62),
        new(145, 46, 152, 46),
        new(93, 47, 97, 48),
        new(130, 47, 130, 62),
        new(133, 47, 133, 62),
        new(145, 47, 151, 47),
        new(129, 48, 129, 50),
        new(134, 48, 136, 61),
        new(141, 48, 141, 57),
        new(145, 48, 150, 48),
        new(94, 49, 97, 49),
        new(140, 49, 140, 58),
        new(145, 49, 149, 50),
        new(95, 50, 97, 50),
        new(137, 50, 138, 59),
        new(96, 51, 97, 52),
        new(101, 51, 104, 56),
        new(139, 51, 139, 59),
        new(145, 51, 147, 52),
        new(105, 52, 105, 59),
        new(116, 52, 117, 65),
        new(124, 52, 125, 65),
        new(97, 53, 97, 53),
        new(106, 53, 110, 59),
        new(114, 53, 115, 61),
        new(145, 53, 146, 53),
        new(145, 54, 145, 54),
        new(111, 55, 111, 61),
        new(126, 55, 129, 61),
        new(112, 56, 112, 61),
        new(142, 56, 142, 56),
        new(102, 57, 104, 57),
        new(113, 57, 113, 61),
        new(103, 58, 104, 58),
        new(104, 59, 104, 59),
        new(107, 60, 110, 60),
        new(137, 60, 137, 60),
        new(110, 61, 110, 61),
        new(126, 62, 126, 62),
        new(134, 62, 135, 62),
        new(104, 66, 104, 66),
        new(38, 83, 38, 83),
        new(37, 84, 37, 84),
        new(36, 85, 36, 85),
        new(35, 86, 35, 95),
        new(34, 87, 34, 87),
        new(34, 91, 34, 91),
        new(34, 93, 34, 95),
        new(152, 105, 152, 105),
        new(151, 119, 152, 119),
        new(153, 120, 153, 121),
        new(228, 123, 231, 129),
        new(232, 124, 232, 132),
        new(227, 125, 227, 127),
        new(233, 125, 233, 132),
        new(234, 129, 234, 132),
        new(229, 130, 231, 131),
        new(157, 133, 157, 133),
        new(158, 134, 158, 135),
        new(159, 135, 159, 135),
        new(110, 155, 110, 155),
        new(229, 155, 229, 155),
        new(90, 161, 90, 161),
        new(91, 162, 91, 162),
        new(141, 165, 141, 166),
        new(140, 167, 140, 167),
        new(137, 178, 137, 178),
        new(136, 179, 136, 179),
        new(205, 193, 205, 193),
        new(202, 194, 202, 194),
        new(227, 194, 227, 195),
        new(193, 196, 193, 196),
        new(162, 235, 163, 235),
        new(110, 238, 112, 238),
        new(116, 238, 117, 238),
        new(112, 239, 112, 239),
        new(116, 239, 116, 239),
        new(101, 252, 110, 253),
        new(96, 253, 100, 253),
        new(111, 253, 111, 253),
        new(141, 253, 151, 253),
    ];

    /// <summary>
    /// Applies the terrain of the occupation state to the terrain of the map.
    /// </summary>
    /// <param name="terrain">The terrain of the crywolf map.</param>
    /// <param name="isPeace">If set to <c>true</c>, the terrain of the peace is applied; otherwise, the one of the war and the occupation.</param>
    public static void Apply(GameMapTerrain terrain, bool isPeace)
    {
        foreach (var (x1, y1, x2, y2) in SafezoneRemovedAreas)
        {
            ForEach(x1, y1, x2, y2, (x, y) => terrain.ApplyTerrainAttribute(x, y, TerrainAttributeType.Safezone, isPeace));
        }

        foreach (var (x1, y1, x2, y2) in BlockedAreas)
        {
            ForEach(x1, y1, x2, y2, (x, y) => terrain.ApplyTerrainAttribute(x, y, TerrainAttributeType.Blocked, !isPeace));
        }
    }

    private static void ForEach(byte x1, byte y1, byte x2, byte y2, Action<byte, byte> action)
    {
        for (var x = x1; x <= x2; x++)
        {
            for (var y = y1; y <= y2; y++)
            {
                action(x, y);
            }
        }
    }
}
