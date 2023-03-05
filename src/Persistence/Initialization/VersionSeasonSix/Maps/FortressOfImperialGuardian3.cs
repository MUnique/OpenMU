// <copyright file="FortressOfImperialGuardian3.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Map initialization for the Empire fortress 3 event map.
/// </summary>
internal class FortressOfImperialGuardian3 : BaseMapInitializer
{
    /// <summary>
    /// The Number of the Map.
    /// </summary>
    internal const byte Number = 71;

    /// <summary>
    /// The Name of the Map.
    /// </summary>
    internal const string Name = "Fortress of Imperial Guardian 3";

    /// <summary>
    /// Initializes a new instance of the <see cref="FortressOfImperialGuardian3"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public FortressOfImperialGuardian3(IContext context, GameConfiguration gameConfiguration)
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
        yield return this.CreateMonsterSpawn(100, this.NpcDictionary[520], 132, 185, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 4
        yield return this.CreateMonsterSpawn(101, this.NpcDictionary[520], 131, 192, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 4
        yield return this.CreateMonsterSpawn(102, this.NpcDictionary[520], 132, 199, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 4
        yield return this.CreateMonsterSpawn(103, this.NpcDictionary[519], 126, 185, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 4
        yield return this.CreateMonsterSpawn(104, this.NpcDictionary[519], 124, 192, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 4
        yield return this.CreateMonsterSpawn(105, this.NpcDictionary[519], 126, 199, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 4
        yield return this.CreateMonsterSpawn(106, this.NpcDictionary[516], 127, 192, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 4
        yield return this.CreateMonsterSpawn(107, this.NpcDictionary[518], 217, 140, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 4
        yield return this.CreateMonsterSpawn(108, this.NpcDictionary[518], 228, 140, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 4
        yield return this.CreateMonsterSpawn(109, this.NpcDictionary[518], 222, 142, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 4
        yield return this.CreateMonsterSpawn(110, this.NpcDictionary[519], 218, 148, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 4
        yield return this.CreateMonsterSpawn(111, this.NpcDictionary[519], 222, 152, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 4
        yield return this.CreateMonsterSpawn(112, this.NpcDictionary[519], 227, 148, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 4
        yield return this.CreateMonsterSpawn(113, this.NpcDictionary[517], 222, 147, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 4
        yield return this.CreateMonsterSpawn(114, this.NpcDictionary[521], 162, 226, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 4
        yield return this.CreateMonsterSpawn(115, this.NpcDictionary[521], 166, 228, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 4
        yield return this.CreateMonsterSpawn(116, this.NpcDictionary[521], 171, 226, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 4
        yield return this.CreateMonsterSpawn(117, this.NpcDictionary[519], 161, 231, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 4
        yield return this.CreateMonsterSpawn(118, this.NpcDictionary[519], 164, 233, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 4
        yield return this.CreateMonsterSpawn(119, this.NpcDictionary[519], 171, 233, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 4
        yield return this.CreateMonsterSpawn(120, this.NpcDictionary[510], 166, 233, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 4
        yield return this.CreateMonsterSpawn(121, this.NpcDictionary[525], 146, 191, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 4
        yield return this.CreateMonsterSpawn(122, this.NpcDictionary[525], 89, 195, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 4
        yield return this.CreateMonsterSpawn(123, this.NpcDictionary[524], 119, 192, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 4
        yield return this.CreateMonsterSpawn(124, this.NpcDictionary[526], 111, 197, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 4
        yield return this.CreateMonsterSpawn(125, this.NpcDictionary[526], 111, 192, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 4
        yield return this.CreateMonsterSpawn(126, this.NpcDictionary[525], 222, 134, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 4
        yield return this.CreateMonsterSpawn(127, this.NpcDictionary[525], 223, 193, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 4
        yield return this.CreateMonsterSpawn(128, this.NpcDictionary[524], 222, 160, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 4
        yield return this.CreateMonsterSpawn(129, this.NpcDictionary[526], 220, 173, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 4
        yield return this.CreateMonsterSpawn(130, this.NpcDictionary[526], 224, 173, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 4
        yield return this.CreateMonsterSpawn(131, this.NpcDictionary[525], 167, 217, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 4
        yield return this.CreateMonsterSpawn(132, this.NpcDictionary[526], 158, 236, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 4
        yield return this.CreateMonsterSpawn(133, this.NpcDictionary[526], 175, 238, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 4
        yield return this.CreateMonsterSpawn(134, this.NpcDictionary[520], 132, 185, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 7
        yield return this.CreateMonsterSpawn(135, this.NpcDictionary[520], 131, 192, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 7
        yield return this.CreateMonsterSpawn(136, this.NpcDictionary[520], 132, 199, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 7
        yield return this.CreateMonsterSpawn(137, this.NpcDictionary[519], 126, 185, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 7
        yield return this.CreateMonsterSpawn(138, this.NpcDictionary[519], 124, 192, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 7
        yield return this.CreateMonsterSpawn(139, this.NpcDictionary[519], 126, 199, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 7
        yield return this.CreateMonsterSpawn(140, this.NpcDictionary[516], 127, 192, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 7
        yield return this.CreateMonsterSpawn(141, this.NpcDictionary[518], 217, 140, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 7
        yield return this.CreateMonsterSpawn(142, this.NpcDictionary[518], 228, 140, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 7
        yield return this.CreateMonsterSpawn(143, this.NpcDictionary[518], 222, 142, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 7
        yield return this.CreateMonsterSpawn(144, this.NpcDictionary[519], 218, 148, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 7
        yield return this.CreateMonsterSpawn(145, this.NpcDictionary[519], 222, 152, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 7
        yield return this.CreateMonsterSpawn(146, this.NpcDictionary[519], 227, 148, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 7
        yield return this.CreateMonsterSpawn(147, this.NpcDictionary[517], 222, 147, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 7
        yield return this.CreateMonsterSpawn(148, this.NpcDictionary[521], 162, 226, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 7
        yield return this.CreateMonsterSpawn(149, this.NpcDictionary[521], 166, 228, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 7
        yield return this.CreateMonsterSpawn(150, this.NpcDictionary[521], 171, 226, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 7
        yield return this.CreateMonsterSpawn(151, this.NpcDictionary[519], 161, 231, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 7
        yield return this.CreateMonsterSpawn(152, this.NpcDictionary[519], 164, 233, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 7
        yield return this.CreateMonsterSpawn(153, this.NpcDictionary[519], 171, 233, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 7
        yield return this.CreateMonsterSpawn(154, this.NpcDictionary[506], 166, 233, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 7
        yield return this.CreateMonsterSpawn(155, this.NpcDictionary[525], 146, 191, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 7
        yield return this.CreateMonsterSpawn(156, this.NpcDictionary[525], 89, 195, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 7
        yield return this.CreateMonsterSpawn(157, this.NpcDictionary[524], 119, 192, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 7
        yield return this.CreateMonsterSpawn(158, this.NpcDictionary[526], 111, 197, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 7
        yield return this.CreateMonsterSpawn(159, this.NpcDictionary[526], 111, 192, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 7
        yield return this.CreateMonsterSpawn(160, this.NpcDictionary[525], 222, 134, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 7
        yield return this.CreateMonsterSpawn(161, this.NpcDictionary[525], 223, 193, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 7
        yield return this.CreateMonsterSpawn(162, this.NpcDictionary[524], 222, 160, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 7
        yield return this.CreateMonsterSpawn(163, this.NpcDictionary[526], 220, 173, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 7
        yield return this.CreateMonsterSpawn(164, this.NpcDictionary[526], 224, 173, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 7
        yield return this.CreateMonsterSpawn(165, this.NpcDictionary[525], 167, 217, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 7
        yield return this.CreateMonsterSpawn(166, this.NpcDictionary[526], 158, 236, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 7
        yield return this.CreateMonsterSpawn(167, this.NpcDictionary[526], 175, 238, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 7

        // Traps:
        yield return this.CreateMonsterSpawn(200, this.NpcDictionary[523], 113, 193, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 4
        yield return this.CreateMonsterSpawn(201, this.NpcDictionary[523], 112, 198, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 4
        yield return this.CreateMonsterSpawn(202, this.NpcDictionary[523], 112, 203, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 4
        yield return this.CreateMonsterSpawn(203, this.NpcDictionary[523], 107, 203, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 4
        yield return this.CreateMonsterSpawn(204, this.NpcDictionary[523], 102, 203, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 4
        yield return this.CreateMonsterSpawn(205, this.NpcDictionary[523], 97, 203, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 4
        yield return this.CreateMonsterSpawn(206, this.NpcDictionary[523], 93, 203, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 4
        yield return this.CreateMonsterSpawn(207, this.NpcDictionary[523], 93, 198, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 4
        yield return this.CreateMonsterSpawn(208, this.NpcDictionary[523], 93, 192, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 4
        yield return this.CreateMonsterSpawn(209, this.NpcDictionary[523], 93, 187, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 4
        yield return this.CreateMonsterSpawn(210, this.NpcDictionary[523], 97, 186, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 4
        yield return this.CreateMonsterSpawn(211, this.NpcDictionary[523], 102, 186, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 4
        yield return this.CreateMonsterSpawn(212, this.NpcDictionary[523], 107, 186, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 4
        yield return this.CreateMonsterSpawn(213, this.NpcDictionary[523], 112, 187, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 4
        yield return this.CreateMonsterSpawn(214, this.NpcDictionary[523], 222, 165, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 4
        yield return this.CreateMonsterSpawn(215, this.NpcDictionary[523], 222, 170, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 4
        yield return this.CreateMonsterSpawn(216, this.NpcDictionary[523], 222, 175, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 4
        yield return this.CreateMonsterSpawn(217, this.NpcDictionary[523], 222, 180, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 4
        yield return this.CreateMonsterSpawn(218, this.NpcDictionary[523], 222, 185, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 4
        yield return this.CreateMonsterSpawn(219, this.NpcDictionary[523], 222, 190, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 4
        yield return this.CreateMonsterSpawn(220, this.NpcDictionary[523], 113, 193, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 7
        yield return this.CreateMonsterSpawn(221, this.NpcDictionary[523], 112, 198, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 7
        yield return this.CreateMonsterSpawn(222, this.NpcDictionary[523], 112, 203, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 7
        yield return this.CreateMonsterSpawn(223, this.NpcDictionary[523], 107, 203, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 7
        yield return this.CreateMonsterSpawn(224, this.NpcDictionary[523], 102, 203, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 7
        yield return this.CreateMonsterSpawn(225, this.NpcDictionary[523], 97, 203, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 7
        yield return this.CreateMonsterSpawn(226, this.NpcDictionary[523], 93, 203, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 7
        yield return this.CreateMonsterSpawn(227, this.NpcDictionary[523], 93, 198, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 7
        yield return this.CreateMonsterSpawn(228, this.NpcDictionary[523], 93, 192, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 7
        yield return this.CreateMonsterSpawn(229, this.NpcDictionary[523], 93, 187, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 7
        yield return this.CreateMonsterSpawn(230, this.NpcDictionary[523], 97, 186, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 7
        yield return this.CreateMonsterSpawn(231, this.NpcDictionary[523], 102, 186, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 7
        yield return this.CreateMonsterSpawn(232, this.NpcDictionary[523], 107, 186, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 7
        yield return this.CreateMonsterSpawn(233, this.NpcDictionary[523], 112, 187, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 7
        yield return this.CreateMonsterSpawn(234, this.NpcDictionary[523], 222, 165, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 7
        yield return this.CreateMonsterSpawn(235, this.NpcDictionary[523], 222, 170, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 7
        yield return this.CreateMonsterSpawn(236, this.NpcDictionary[523], 222, 175, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 7
        yield return this.CreateMonsterSpawn(237, this.NpcDictionary[523], 222, 180, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 7
        yield return this.CreateMonsterSpawn(238, this.NpcDictionary[523], 222, 185, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 7
        yield return this.CreateMonsterSpawn(239, this.NpcDictionary[523], 222, 190, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 7
    }
}