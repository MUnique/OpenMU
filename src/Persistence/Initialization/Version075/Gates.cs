// <copyright file="Gates.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version075;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Gates initialization.
/// </summary>
public class Gates : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Gates" /> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Gates(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <summary>
    /// Initializes the gates.
    /// </summary>
    public override void Initialize()
    {
        var maps = this.GameConfiguration.Maps.ToDictionary(map => map.Number, map => map);
        var targetGates = this.CreateTargetGates(maps);
        this.CreateEnterGates(maps, targetGates);
        this.CreateWarpEntries(targetGates);
    }

    /// <summary>
    /// Creates the warp entries.
    /// </summary>
    /// <param name="gates">The gates.</param>
    /// <remarks>
    /// Version 0.75 has no graphical menu, but allows to warp with text commands.
    /// </remarks>
    private void CreateWarpEntries(IDictionary<short, ExitGate> gates)
    {
        this.GameConfiguration.WarpList.Add(this.CreateWarpInfo(1, "Arena", 2000, 50, gates[50]));
        this.GameConfiguration.WarpList.Add(this.CreateWarpInfo(2, "Lorencia", 2000, 10, gates[17]));
        this.GameConfiguration.WarpList.Add(this.CreateWarpInfo(3, "Noria", 2000, 10, gates[27]));
        this.GameConfiguration.WarpList.Add(this.CreateWarpInfo(4, "Devias", 2000, 20, gates[22]));
        this.GameConfiguration.WarpList.Add(this.CreateWarpInfo(5, "Dungeon", 3000, 30, gates[2]));
        this.GameConfiguration.WarpList.Add(this.CreateWarpInfo(6, "Dungeon2", 3500, 40, gates[6]));
        this.GameConfiguration.WarpList.Add(this.CreateWarpInfo(7, "Dungeon3", 4000, 50, gates[10]));
        this.GameConfiguration.WarpList.Add(this.CreateWarpInfo(8, "LostTower", 5000, 50, gates[42]));
        this.GameConfiguration.WarpList.Add(this.CreateWarpInfo(9, "LostTower2", 5500, 50, gates[31]));
        this.GameConfiguration.WarpList.Add(this.CreateWarpInfo(10, "LostTower3", 6000, 50, gates[33]));
        this.GameConfiguration.WarpList.Add(this.CreateWarpInfo(11, "LostTower4", 6500, 60, gates[35]));
        this.GameConfiguration.WarpList.Add(this.CreateWarpInfo(12, "LostTower5", 7000, 60, gates[37]));
        this.GameConfiguration.WarpList.Add(this.CreateWarpInfo(13, "LostTower6", 7500, 70, gates[39]));
        this.GameConfiguration.WarpList.Add(this.CreateWarpInfo(14, "LostTower7", 8000, 70, gates[41]));
    }

    private WarpInfo CreateWarpInfo(ushort index, string name, int costs, int levelRequirement, ExitGate gate)
    {
        var warpInfo = this.Context.CreateNew<WarpInfo>();
        warpInfo.Index = index;
        warpInfo.Name = name;
        warpInfo.Costs = costs;
        warpInfo.LevelRequirement = levelRequirement;
        warpInfo.Gate = gate;
        return warpInfo;
    }

    private ExitGate CreateExitGate(GameMapDefinition map, byte x1, byte y1, byte x2, byte y2, byte direction, bool isSpawnGate = false)
    {
        if (x1 > x2)
        {
            throw new ArgumentException("x1 > x2");
        }

        if (y1 > y2)
        {
            throw new ArgumentException("y1 > y2");
        }

        var gate = this.Context.CreateNew<ExitGate>();
        gate.Map = map;
        gate.X1 = x1;
        gate.Y1 = y1;
        gate.X2 = x2;
        gate.Y2 = y2;

        // different to all other configurations, 0 means 'Undefined', so we just assume that we can cast it to Direction without adding 1.
        gate.Direction = (Direction)direction;
        gate.IsSpawnGate = isSpawnGate;
        map.ExitGates.Add(gate);
        return gate;
    }

    /// <summary>
    /// Creates the target gates and adds them to the <see cref="GameMapDefinition.ExitGates" /> if the gate is specified as spawn gate (flag == 0).
    /// </summary>
    /// <param name="maps">The previously created game maps.</param>
    /// <returns>A dictionary of all created exit gates. The key is the number.</returns>
    /// <remarks>
    /// Regex #1: (?m)^(\d+)\s+?(0)\s+?(\d+)\s+?(\d+)\s+?(\d+)\s+?(\d+)\s+?(\d+)\s+?(\d+)\s+?(\d+)\s+?(\d+).*?$
    /// Rplace by #1: targetGates.Add($1, this.CreateExitGate(maps[$3], $4, $5, $6, $7, $9, true));
    /// Regex #2: (?m)^(\d+)\s+?(2)\s+?(\d+)\s+?(\d+)\s+?(\d+)\s+?(\d+)\s+?(\d+)\s+?(\d+)\s+?(\d+)\s+?(\d+).*?$
    /// Rplace by #2: targetGates.Add($1, this.CreateExitGate(maps[$3], $4, $5, $6, $7, $9, false));.
    /// </remarks>
    private IDictionary<short, ExitGate> CreateTargetGates(IDictionary<short, GameMapDefinition> maps)
    {
        var targetGates = new Dictionary<short, ExitGate>();

        // Lorencia
        targetGates.Add(17, this.CreateExitGate(maps[0], 133, 118, 151, 135, 0, true));
        targetGates.Add(4, this.CreateExitGate(maps[0], 121, 231, 123, 231, 1));
        targetGates.Add(21, this.CreateExitGate(maps[0], 7, 38, 8, 41, 3));
        targetGates.Add(26, this.CreateExitGate(maps[0], 213, 244, 217, 245, 1));

        // Dungeon
        targetGates.Add(2, this.CreateExitGate(maps[1], 107, 247, 110, 247, 1));
        targetGates.Add(6, this.CreateExitGate(maps[1], 231, 126, 234, 127, 1));
        targetGates.Add(8, this.CreateExitGate(maps[1], 240, 149, 241, 151, 3));
        targetGates.Add(10, this.CreateExitGate(maps[1], 3, 83, 4, 86, 3));
        targetGates.Add(12, this.CreateExitGate(maps[1], 3, 16, 6, 17, 3));
        targetGates.Add(14, this.CreateExitGate(maps[1], 29, 125, 30, 126, 1));
        targetGates.Add(16, this.CreateExitGate(maps[1], 5, 32, 7, 33, 1));

        // Devias
        targetGates.Add(22, this.CreateExitGate(maps[2], 197, 35, 218, 50, 0, true));
        targetGates.Add(19, this.CreateExitGate(maps[2], 242, 34, 243, 37, 7));
        targetGates.Add(44, this.CreateExitGate(maps[2], 2, 246, 3, 247, 2));

        // Noria
        targetGates.Add(27, this.CreateExitGate(maps[3], 171, 108, 177, 117, 0, true));
        targetGates.Add(24, this.CreateExitGate(maps[3], 148, 5, 155, 6, 5));
        targetGates.Add(48, this.CreateExitGate(maps[3], 240, 240, 241, 243, 7));

        // Lost Tower
        targetGates.Add(42, this.CreateExitGate(maps[4], 203, 70, 213, 81, 0, true));
        targetGates.Add(29, this.CreateExitGate(maps[4], 162, 2, 166, 3, 5));
        targetGates.Add(31, this.CreateExitGate(maps[4], 241, 237, 244, 238, 1));
        targetGates.Add(33, this.CreateExitGate(maps[4], 86, 166, 87, 168, 3));
        targetGates.Add(35, this.CreateExitGate(maps[4], 87, 86, 88, 89, 3));
        targetGates.Add(37, this.CreateExitGate(maps[4], 128, 53, 131, 54, 1));
        targetGates.Add(39, this.CreateExitGate(maps[4], 52, 53, 55, 54, 1));
        targetGates.Add(41, this.CreateExitGate(maps[4], 8, 85, 9, 87, 1));

        // Atlans
        targetGates.Add(49, this.CreateExitGate(maps[7], 15, 11, 27, 23, 0, true));
        targetGates.Add(46, this.CreateExitGate(maps[7], 14, 12, 15, 13, 3));

        // Arena
        targetGates.Add(50, this.CreateExitGate(maps[6], 101, 115, 103, 117, 0, true));
        targetGates.Add(51, this.CreateExitGate(maps[6], 107, 115, 107, 115, 0, true));
        targetGates.Add(52, this.CreateExitGate(maps[6], 107, 114, 107, 114, 0, true));

        return targetGates;
    }

    private EnterGate CreateEnterGate(short number, ExitGate targetGate, byte x1, byte y1, byte x2, byte y2, short levelRequirement)
    {
        var enterGate = this.Context.CreateNew<EnterGate>();
        enterGate.Number = number;
        enterGate.LevelRequirement = levelRequirement;
        enterGate.TargetGate = targetGate;
        enterGate.X1 = x1;
        enterGate.Y1 = y1;
        enterGate.X2 = x2;
        enterGate.Y2 = y2;
        enterGate.LevelRequirement = levelRequirement;
        return enterGate;
    }

    /// <summary>
    /// Creates the enter gates for all maps.
    /// </summary>
    /// <remarks>
    /// Regex: (?m)^(\d+)\s+?(1)\s+?(\d+)\s+?(\d+)\s+?(\d+)\s+?(\d+)\s+?(\d+)\s+?(\d+)
    /// Replace by #2: maps[$3].EnterGates.Add(this.CreateEnterGate($1, targetGates[$8], $4, $5, $6, $7, $10));
    /// Remove other lines with empty replacement and regex: (?m)^\d+\s+.*$.
    /// </remarks>
    /// <param name="maps">The maps.</param>
    /// <param name="targetGates">The target gates by number.</param>
    private void CreateEnterGates(IDictionary<short, GameMapDefinition> maps, IDictionary<short, ExitGate> targetGates)
    {
        maps[0].EnterGates.Add(this.CreateEnterGate(1, targetGates[2], 121, 232, 123, 233, 20));
        maps[1].EnterGates.Add(this.CreateEnterGate(3, targetGates[4], 108, 248, 109, 248, 0));
        maps[1].EnterGates.Add(this.CreateEnterGate(5, targetGates[6], 239, 149, 239, 150, 20));
        maps[1].EnterGates.Add(this.CreateEnterGate(7, targetGates[8], 232, 127, 233, 128, 20));
        maps[1].EnterGates.Add(this.CreateEnterGate(9, targetGates[10], 2, 17, 2, 18, 20));
        maps[1].EnterGates.Add(this.CreateEnterGate(11, targetGates[12], 2, 84, 2, 85, 20));
        maps[1].EnterGates.Add(this.CreateEnterGate(13, targetGates[14], 5, 34, 6, 34, 20));
        maps[1].EnterGates.Add(this.CreateEnterGate(15, targetGates[16], 29, 127, 30, 127, 20));
        maps[0].EnterGates.Add(this.CreateEnterGate(18, targetGates[19], 5, 38, 6, 41, 15));
        maps[2].EnterGates.Add(this.CreateEnterGate(20, targetGates[21], 244, 34, 245, 37, 0));
        maps[0].EnterGates.Add(this.CreateEnterGate(23, targetGates[24], 213, 246, 217, 247, 10));
        maps[3].EnterGates.Add(this.CreateEnterGate(25, targetGates[26], 148, 3, 155, 4, 10));
        maps[2].EnterGates.Add(this.CreateEnterGate(28, targetGates[29], 2, 248, 3, 249, 40));
        maps[4].EnterGates.Add(this.CreateEnterGate(30, targetGates[31], 190, 6, 191, 8, 40));
        maps[4].EnterGates.Add(this.CreateEnterGate(32, targetGates[33], 166, 163, 167, 166, 40));
        maps[4].EnterGates.Add(this.CreateEnterGate(34, targetGates[35], 132, 245, 135, 246, 50));
        maps[4].EnterGates.Add(this.CreateEnterGate(36, targetGates[37], 132, 135, 135, 136, 50));
        maps[4].EnterGates.Add(this.CreateEnterGate(38, targetGates[39], 131, 15, 132, 18, 50));
        maps[4].EnterGates.Add(this.CreateEnterGate(40, targetGates[41], 6, 5, 7, 8, 50));
        maps[4].EnterGates.Add(this.CreateEnterGate(43, targetGates[44], 162, 0, 166, 1, 15));
        maps[3].EnterGates.Add(this.CreateEnterGate(45, targetGates[46], 242, 240, 245, 243, 60));
        maps[7].EnterGates.Add(this.CreateEnterGate(47, targetGates[48], 9, 9, 11, 12, 60));
    }
}