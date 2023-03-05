// <copyright file="FortressOfImperialGuardian2.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Map initialization for the Empire fortress 2 event map.
/// </summary>
internal class FortressOfImperialGuardian2 : BaseMapInitializer
{
    /// <summary>
    /// The Number of the Map.
    /// </summary>
    internal const byte Number = 70;

    /// <summary>
    /// The Name of the Map.
    /// </summary>
    internal const string Name = "Fortress of Imperial Guardian 2";

    /// <summary>
    /// Initializes a new instance of the <see cref="FortressOfImperialGuardian2"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public FortressOfImperialGuardian2(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    protected override byte MapNumber => Number;

    /// <inheritdoc />
    protected override string MapName => Name;

    /// <inheritdoc />
    protected override void CreateMonsters()
    {
        // All Monsters and NPCs are defined in the first map.
    }

    /// <inheritdoc />
    protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        yield return this.CreateMonsterSpawn(100, this.NpcDictionary[520], 66, 59, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 3
        yield return this.CreateMonsterSpawn(101, this.NpcDictionary[520], 64, 65, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 3
        yield return this.CreateMonsterSpawn(102, this.NpcDictionary[520], 66, 70, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 3
        yield return this.CreateMonsterSpawn(103, this.NpcDictionary[519], 56, 58, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 3
        yield return this.CreateMonsterSpawn(104, this.NpcDictionary[519], 56, 65, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 3
        yield return this.CreateMonsterSpawn(105, this.NpcDictionary[519], 56, 71, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 3
        yield return this.CreateMonsterSpawn(106, this.NpcDictionary[514], 59, 65, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 3
        yield return this.CreateMonsterSpawn(107, this.NpcDictionary[518], 32, 99, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 3
        yield return this.CreateMonsterSpawn(108, this.NpcDictionary[518], 42, 99, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 3
        yield return this.CreateMonsterSpawn(109, this.NpcDictionary[518], 38, 101, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 3
        yield return this.CreateMonsterSpawn(110, this.NpcDictionary[519], 33, 107, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 3
        yield return this.CreateMonsterSpawn(111, this.NpcDictionary[519], 44, 107, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 3
        yield return this.CreateMonsterSpawn(112, this.NpcDictionary[519], 39, 110, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 3
        yield return this.CreateMonsterSpawn(113, this.NpcDictionary[515], 38, 106, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 3
        yield return this.CreateMonsterSpawn(114, this.NpcDictionary[521], 98, 107, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 3
        yield return this.CreateMonsterSpawn(115, this.NpcDictionary[521], 96, 112, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 3
        yield return this.CreateMonsterSpawn(116, this.NpcDictionary[521], 98, 117, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 3
        yield return this.CreateMonsterSpawn(117, this.NpcDictionary[519], 93, 105, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 3
        yield return this.CreateMonsterSpawn(118, this.NpcDictionary[519], 92, 112, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 3
        yield return this.CreateMonsterSpawn(119, this.NpcDictionary[519], 93, 118, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 3
        yield return this.CreateMonsterSpawn(120, this.NpcDictionary[509], 89, 111, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 3
        yield return this.CreateMonsterSpawn(121, this.NpcDictionary[525], 75, 67, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 3
        yield return this.CreateMonsterSpawn(122, this.NpcDictionary[525], 19, 65, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 3
        yield return this.CreateMonsterSpawn(123, this.NpcDictionary[524], 50, 65, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 3
        yield return this.CreateMonsterSpawn(124, this.NpcDictionary[526], 24, 67, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 3
        yield return this.CreateMonsterSpawn(125, this.NpcDictionary[526], 24, 62, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 3
        yield return this.CreateMonsterSpawn(126, this.NpcDictionary[525], 37, 93, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 3
        yield return this.CreateMonsterSpawn(127, this.NpcDictionary[525], 55, 154, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 3
        yield return this.CreateMonsterSpawn(128, this.NpcDictionary[524], 41, 117, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 3
        yield return this.CreateMonsterSpawn(129, this.NpcDictionary[526], 53, 152, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 3
        yield return this.CreateMonsterSpawn(130, this.NpcDictionary[526], 56, 152, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 3
        yield return this.CreateMonsterSpawn(131, this.NpcDictionary[525], 107, 112, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 3
        yield return this.CreateMonsterSpawn(132, this.NpcDictionary[526], 85, 113, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 3
        yield return this.CreateMonsterSpawn(133, this.NpcDictionary[526], 85, 107, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 3
        yield return this.CreateMonsterSpawn(134, this.NpcDictionary[520], 66, 59, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 6
        yield return this.CreateMonsterSpawn(135, this.NpcDictionary[520], 64, 65, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 6
        yield return this.CreateMonsterSpawn(136, this.NpcDictionary[520], 66, 70, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 6
        yield return this.CreateMonsterSpawn(137, this.NpcDictionary[519], 56, 58, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 6
        yield return this.CreateMonsterSpawn(138, this.NpcDictionary[519], 56, 65, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 6
        yield return this.CreateMonsterSpawn(139, this.NpcDictionary[519], 56, 71, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 6
        yield return this.CreateMonsterSpawn(140, this.NpcDictionary[514], 59, 65, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 6
        yield return this.CreateMonsterSpawn(141, this.NpcDictionary[518], 32, 99, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 6
        yield return this.CreateMonsterSpawn(142, this.NpcDictionary[518], 42, 99, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 6
        yield return this.CreateMonsterSpawn(143, this.NpcDictionary[518], 38, 101, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 6
        yield return this.CreateMonsterSpawn(144, this.NpcDictionary[519], 33, 107, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 6
        yield return this.CreateMonsterSpawn(145, this.NpcDictionary[519], 44, 107, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 6
        yield return this.CreateMonsterSpawn(146, this.NpcDictionary[519], 39, 110, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 6
        yield return this.CreateMonsterSpawn(147, this.NpcDictionary[515], 38, 106, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 6
        yield return this.CreateMonsterSpawn(148, this.NpcDictionary[521], 98, 107, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 6
        yield return this.CreateMonsterSpawn(149, this.NpcDictionary[521], 96, 112, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 6
        yield return this.CreateMonsterSpawn(150, this.NpcDictionary[521], 98, 117, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 6
        yield return this.CreateMonsterSpawn(151, this.NpcDictionary[519], 93, 105, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 6
        yield return this.CreateMonsterSpawn(152, this.NpcDictionary[519], 92, 112, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 6
        yield return this.CreateMonsterSpawn(153, this.NpcDictionary[519], 93, 118, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 6
        yield return this.CreateMonsterSpawn(154, this.NpcDictionary[507], 89, 111, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 6
        yield return this.CreateMonsterSpawn(155, this.NpcDictionary[525], 75, 67, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 6
        yield return this.CreateMonsterSpawn(156, this.NpcDictionary[525], 19, 65, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 6
        yield return this.CreateMonsterSpawn(157, this.NpcDictionary[524], 50, 65, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 6
        yield return this.CreateMonsterSpawn(158, this.NpcDictionary[526], 24, 67, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 6
        yield return this.CreateMonsterSpawn(159, this.NpcDictionary[526], 24, 62, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 6
        yield return this.CreateMonsterSpawn(160, this.NpcDictionary[525], 37, 93, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 6
        yield return this.CreateMonsterSpawn(161, this.NpcDictionary[525], 55, 154, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 6
        yield return this.CreateMonsterSpawn(162, this.NpcDictionary[524], 41, 117, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 6
        yield return this.CreateMonsterSpawn(163, this.NpcDictionary[526], 53, 152, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 6
        yield return this.CreateMonsterSpawn(164, this.NpcDictionary[526], 56, 152, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 6
        yield return this.CreateMonsterSpawn(165, this.NpcDictionary[525], 107, 112, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 6
        yield return this.CreateMonsterSpawn(166, this.NpcDictionary[526], 85, 113, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 6
        yield return this.CreateMonsterSpawn(167, this.NpcDictionary[526], 85, 107, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 6

        // Traps:
        yield return this.CreateMonsterSpawn(200, this.NpcDictionary[523], 45, 67, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 3
        yield return this.CreateMonsterSpawn(201, this.NpcDictionary[523], 42, 62, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 3
        yield return this.CreateMonsterSpawn(202, this.NpcDictionary[523], 39, 67, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 3
        yield return this.CreateMonsterSpawn(203, this.NpcDictionary[523], 35, 62, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 3
        yield return this.CreateMonsterSpawn(204, this.NpcDictionary[523], 32, 67, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 3
        yield return this.CreateMonsterSpawn(205, this.NpcDictionary[523], 29, 62, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 3
        yield return this.CreateMonsterSpawn(206, this.NpcDictionary[523], 26, 67, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 3
        yield return this.CreateMonsterSpawn(207, this.NpcDictionary[523], 24, 62, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 3
        yield return this.CreateMonsterSpawn(208, this.NpcDictionary[523], 22, 66, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 3
        yield return this.CreateMonsterSpawn(209, this.NpcDictionary[523], 41, 122, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 3
        yield return this.CreateMonsterSpawn(210, this.NpcDictionary[523], 41, 126, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 3
        yield return this.CreateMonsterSpawn(211, this.NpcDictionary[523], 41, 130, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 3
        yield return this.CreateMonsterSpawn(212, this.NpcDictionary[523], 45, 128, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 3
        yield return this.CreateMonsterSpawn(213, this.NpcDictionary[523], 50, 128, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 3
        yield return this.CreateMonsterSpawn(214, this.NpcDictionary[523], 55, 129, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 3
        yield return this.CreateMonsterSpawn(215, this.NpcDictionary[523], 55, 134, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 3
        yield return this.CreateMonsterSpawn(216, this.NpcDictionary[523], 55, 139, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 3
        yield return this.CreateMonsterSpawn(217, this.NpcDictionary[523], 55, 144, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 3
        yield return this.CreateMonsterSpawn(218, this.NpcDictionary[523], 55, 149, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 3
        yield return this.CreateMonsterSpawn(219, this.NpcDictionary[523], 45, 67, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 6
        yield return this.CreateMonsterSpawn(220, this.NpcDictionary[523], 42, 62, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 6
        yield return this.CreateMonsterSpawn(221, this.NpcDictionary[523], 39, 67, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 6
        yield return this.CreateMonsterSpawn(222, this.NpcDictionary[523], 35, 62, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 6
        yield return this.CreateMonsterSpawn(223, this.NpcDictionary[523], 32, 67, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 6
        yield return this.CreateMonsterSpawn(224, this.NpcDictionary[523], 29, 62, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 6
        yield return this.CreateMonsterSpawn(225, this.NpcDictionary[523], 26, 67, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 6
        yield return this.CreateMonsterSpawn(226, this.NpcDictionary[523], 24, 62, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 6
        yield return this.CreateMonsterSpawn(227, this.NpcDictionary[523], 22, 66, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 6
        yield return this.CreateMonsterSpawn(228, this.NpcDictionary[523], 41, 122, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 6
        yield return this.CreateMonsterSpawn(229, this.NpcDictionary[523], 41, 126, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 6
        yield return this.CreateMonsterSpawn(230, this.NpcDictionary[523], 41, 130, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 6
        yield return this.CreateMonsterSpawn(231, this.NpcDictionary[523], 45, 128, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 6
        yield return this.CreateMonsterSpawn(232, this.NpcDictionary[523], 50, 128, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 6
        yield return this.CreateMonsterSpawn(233, this.NpcDictionary[523], 55, 129, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 6
        yield return this.CreateMonsterSpawn(234, this.NpcDictionary[523], 55, 134, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 6
        yield return this.CreateMonsterSpawn(235, this.NpcDictionary[523], 55, 139, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 6
        yield return this.CreateMonsterSpawn(236, this.NpcDictionary[523], 55, 144, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 6
        yield return this.CreateMonsterSpawn(237, this.NpcDictionary[523], 55, 149, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 6
    }
}