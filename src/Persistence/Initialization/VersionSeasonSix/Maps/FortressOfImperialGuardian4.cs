// <copyright file="FortressOfImperialGuardian4.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Map initialization for the Empire fortress 4 event map.
/// </summary>
internal class FortressOfImperialGuardian4 : BaseMapInitializer
{
    /// <summary>
    /// The Number of the Map.
    /// </summary>
    internal const byte Number = 72;

    /// <summary>
    /// The Name of the Map.
    /// </summary>
    internal const string Name = "Fortress of Imperial Guardian 4";

    /// <summary>
    /// Initializes a new instance of the <see cref="FortressOfImperialGuardian4"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public FortressOfImperialGuardian4(IContext context, GameConfiguration gameConfiguration)
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
        yield return this.CreateMonsterSpawn(100, this.NpcDictionary[518], 64, 65, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 1
        yield return this.CreateMonsterSpawn(101, this.NpcDictionary[518], 64, 69, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 1
        yield return this.CreateMonsterSpawn(102, this.NpcDictionary[518], 64, 73, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 1
        yield return this.CreateMonsterSpawn(103, this.NpcDictionary[519], 61, 64, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 1
        yield return this.CreateMonsterSpawn(104, this.NpcDictionary[519], 61, 69, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 1
        yield return this.CreateMonsterSpawn(105, this.NpcDictionary[519], 61, 74, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 1
        yield return this.CreateMonsterSpawn(106, this.NpcDictionary[521], 55, 71, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 1
        yield return this.CreateMonsterSpawn(107, this.NpcDictionary[521], 55, 66, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 1
        yield return this.CreateMonsterSpawn(108, this.NpcDictionary[521], 54, 68, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 1
        yield return this.CreateMonsterSpawn(109, this.NpcDictionary[507], 57, 69, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 1
        yield return this.CreateMonsterSpawn(110, this.NpcDictionary[518], 29, 186, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 1
        yield return this.CreateMonsterSpawn(111, this.NpcDictionary[518], 34, 187, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 1
        yield return this.CreateMonsterSpawn(112, this.NpcDictionary[518], 39, 186, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 1
        yield return this.CreateMonsterSpawn(113, this.NpcDictionary[519], 34, 194, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 1
        yield return this.CreateMonsterSpawn(114, this.NpcDictionary[519], 29, 192, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 1
        yield return this.CreateMonsterSpawn(115, this.NpcDictionary[519], 39, 192, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 1
        yield return this.CreateMonsterSpawn(116, this.NpcDictionary[521], 26, 191, (Direction)2, SpawnTrigger.OnceAtEventStart); // 1 1
        yield return this.CreateMonsterSpawn(117, this.NpcDictionary[521], 32, 195, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 1
        yield return this.CreateMonsterSpawn(118, this.NpcDictionary[521], 39, 196, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 1
        yield return this.CreateMonsterSpawn(119, this.NpcDictionary[506], 36, 191, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 1
        yield return this.CreateMonsterSpawn(120, this.NpcDictionary[518], 173, 129, (Direction)7, SpawnTrigger.OnceAtEventStart); // 2 1
        yield return this.CreateMonsterSpawn(121, this.NpcDictionary[518], 170, 133, (Direction)7, SpawnTrigger.OnceAtEventStart); // 2 1
        yield return this.CreateMonsterSpawn(122, this.NpcDictionary[518], 173, 137, (Direction)7, SpawnTrigger.OnceAtEventStart); // 2 1
        yield return this.CreateMonsterSpawn(123, this.NpcDictionary[519], 181, 129, (Direction)7, SpawnTrigger.OnceAtEventStart); // 2 1
        yield return this.CreateMonsterSpawn(124, this.NpcDictionary[519], 181, 133, (Direction)7, SpawnTrigger.OnceAtEventStart); // 2 1
        yield return this.CreateMonsterSpawn(125, this.NpcDictionary[519], 181, 137, (Direction)7, SpawnTrigger.OnceAtEventStart); // 2 1
        yield return this.CreateMonsterSpawn(126, this.NpcDictionary[521], 184, 129, (Direction)7, SpawnTrigger.OnceAtEventStart); // 2 1
        yield return this.CreateMonsterSpawn(127, this.NpcDictionary[521], 184, 133, (Direction)7, SpawnTrigger.OnceAtEventStart); // 2 1
        yield return this.CreateMonsterSpawn(128, this.NpcDictionary[521], 184, 137, (Direction)7, SpawnTrigger.OnceAtEventStart); // 2 1
        yield return this.CreateMonsterSpawn(129, this.NpcDictionary[505], 177, 133, (Direction)7, SpawnTrigger.OnceAtEventStart); // 2 1
        yield return this.CreateMonsterSpawn(130, this.NpcDictionary[521], 183, 29, (Direction)3, SpawnTrigger.OnceAtEventStart); // 3 1
        yield return this.CreateMonsterSpawn(131, this.NpcDictionary[521], 183, 21, (Direction)3, SpawnTrigger.OnceAtEventStart); // 3 1
        yield return this.CreateMonsterSpawn(132, this.NpcDictionary[521], 187, 32, (Direction)1, SpawnTrigger.OnceAtEventStart); // 3 1
        yield return this.CreateMonsterSpawn(133, this.NpcDictionary[521], 187, 20, (Direction)5, SpawnTrigger.OnceAtEventStart); // 3 1
        yield return this.CreateMonsterSpawn(134, this.NpcDictionary[518], 192, 31, (Direction)3, SpawnTrigger.OnceAtEventStart); // 3 1
        yield return this.CreateMonsterSpawn(135, this.NpcDictionary[518], 192, 21, (Direction)3, SpawnTrigger.OnceAtEventStart); // 3 1
        yield return this.CreateMonsterSpawn(136, this.NpcDictionary[518], 194, 26, (Direction)3, SpawnTrigger.OnceAtEventStart); // 3 1
        yield return this.CreateMonsterSpawn(137, this.NpcDictionary[519], 189, 21, (Direction)3, SpawnTrigger.OnceAtEventStart); // 3 1
        yield return this.CreateMonsterSpawn(138, this.NpcDictionary[519], 189, 26, (Direction)3, SpawnTrigger.OnceAtEventStart); // 3 1
        yield return this.CreateMonsterSpawn(139, this.NpcDictionary[519], 189, 31, (Direction)3, SpawnTrigger.OnceAtEventStart); // 3 1
        yield return this.CreateMonsterSpawn(140, this.NpcDictionary[504], 183, 24, (Direction)3, SpawnTrigger.OnceAtEventStart); // 3 1
        yield return this.CreateMonsterSpawn(141, this.NpcDictionary[528], 81, 69, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 1
        yield return this.CreateMonsterSpawn(142, this.NpcDictionary[528], 32, 90, (Direction)1, SpawnTrigger.OnceAtEventStart); // 0 1
        yield return this.CreateMonsterSpawn(143, this.NpcDictionary[527], 50, 69, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 1
        yield return this.CreateMonsterSpawn(144, this.NpcDictionary[526], 52, 77, (Direction)2, SpawnTrigger.OnceAtEventStart); // 0 1
        yield return this.CreateMonsterSpawn(145, this.NpcDictionary[526], 53, 61, (Direction)4, SpawnTrigger.OnceAtEventStart); // 0 1
        yield return this.CreateMonsterSpawn(146, this.NpcDictionary[528], 34, 176, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 1
        yield return this.CreateMonsterSpawn(147, this.NpcDictionary[528], 69, 166, (Direction)5, SpawnTrigger.OnceAtEventStart); // 1 1
        yield return this.CreateMonsterSpawn(148, this.NpcDictionary[527], 52, 191, (Direction)7, SpawnTrigger.OnceAtEventStart); // 1 1
        yield return this.CreateMonsterSpawn(149, this.NpcDictionary[526], 21, 198, (Direction)2, SpawnTrigger.OnceAtEventStart); // 1 1
        yield return this.CreateMonsterSpawn(150, this.NpcDictionary[526], 47, 199, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 1
        yield return this.CreateMonsterSpawn(151, this.NpcDictionary[528], 156, 132, (Direction)7, SpawnTrigger.OnceAtEventStart); // 2 1
        yield return this.CreateMonsterSpawn(152, this.NpcDictionary[528], 224, 159, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 1
        yield return this.CreateMonsterSpawn(153, this.NpcDictionary[527], 197, 132, (Direction)7, SpawnTrigger.OnceAtEventStart); // 2 1
        yield return this.CreateMonsterSpawn(154, this.NpcDictionary[526], 161, 139, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 1
        yield return this.CreateMonsterSpawn(155, this.NpcDictionary[526], 161, 127, (Direction)5, SpawnTrigger.OnceAtEventStart); // 2 1
        yield return this.CreateMonsterSpawn(156, this.NpcDictionary[528], 214, 24, (Direction)3, SpawnTrigger.OnceAtEventStart); // 3 1
        yield return this.CreateMonsterSpawn(157, this.NpcDictionary[526], 207, 32, (Direction)1, SpawnTrigger.OnceAtEventStart); // 3 1
        yield return this.CreateMonsterSpawn(158, this.NpcDictionary[526], 207, 20, (Direction)5, SpawnTrigger.OnceAtEventStart); // 3 1

        // Traps
        yield return this.CreateMonsterSpawn(200, this.NpcDictionary[523], 45, 67, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 1
        yield return this.CreateMonsterSpawn(201, this.NpcDictionary[523], 45, 74, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 1
        yield return this.CreateMonsterSpawn(202, this.NpcDictionary[523], 41, 74, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 1
        yield return this.CreateMonsterSpawn(203, this.NpcDictionary[523], 41, 67, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 1
        yield return this.CreateMonsterSpawn(204, this.NpcDictionary[523], 37, 67, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 1
        yield return this.CreateMonsterSpawn(205, this.NpcDictionary[523], 37, 74, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 1
        yield return this.CreateMonsterSpawn(206, this.NpcDictionary[523], 33, 67, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 1
        yield return this.CreateMonsterSpawn(207, this.NpcDictionary[523], 33, 74, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 1
        yield return this.CreateMonsterSpawn(208, this.NpcDictionary[523], 29, 67, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 1
        yield return this.CreateMonsterSpawn(209, this.NpcDictionary[523], 29, 74, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 1
        yield return this.CreateMonsterSpawn(210, this.NpcDictionary[523], 31, 78, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 1
        yield return this.CreateMonsterSpawn(211, this.NpcDictionary[523], 31, 83, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 1
        yield return this.CreateMonsterSpawn(212, this.NpcDictionary[523], 31, 87, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 1
        yield return this.CreateMonsterSpawn(213, this.NpcDictionary[523], 57, 191, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 1
        yield return this.CreateMonsterSpawn(214, this.NpcDictionary[523], 61, 191, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 1
        yield return this.CreateMonsterSpawn(215, this.NpcDictionary[523], 65, 191, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 1
        yield return this.CreateMonsterSpawn(216, this.NpcDictionary[523], 69, 191, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 1
        yield return this.CreateMonsterSpawn(217, this.NpcDictionary[523], 68, 186, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 1
        yield return this.CreateMonsterSpawn(218, this.NpcDictionary[523], 68, 181, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 1
        yield return this.CreateMonsterSpawn(219, this.NpcDictionary[523], 68, 176, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 1
        yield return this.CreateMonsterSpawn(220, this.NpcDictionary[523], 68, 171, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 1
        yield return this.CreateMonsterSpawn(221, this.NpcDictionary[523], 202, 133, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 2 1
        yield return this.CreateMonsterSpawn(222, this.NpcDictionary[523], 206, 133, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 2 1
        yield return this.CreateMonsterSpawn(223, this.NpcDictionary[523], 210, 133, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 2 1
        yield return this.CreateMonsterSpawn(224, this.NpcDictionary[523], 214, 133, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 2 1
        yield return this.CreateMonsterSpawn(225, this.NpcDictionary[523], 218, 133, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 2 1
        yield return this.CreateMonsterSpawn(226, this.NpcDictionary[523], 222, 133, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 2 1
        yield return this.CreateMonsterSpawn(227, this.NpcDictionary[523], 226, 133, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 2 1
        yield return this.CreateMonsterSpawn(228, this.NpcDictionary[523], 224, 137, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 2 1
        yield return this.CreateMonsterSpawn(229, this.NpcDictionary[523], 224, 142, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 2 1
        yield return this.CreateMonsterSpawn(230, this.NpcDictionary[523], 224, 147, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 2 1
        yield return this.CreateMonsterSpawn(231, this.NpcDictionary[523], 224, 152, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 2 1
        yield return this.CreateMonsterSpawn(232, this.NpcDictionary[523], 224, 155, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 2 1
    }
}