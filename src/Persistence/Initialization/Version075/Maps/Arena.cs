﻿// <copyright file="Arena.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version075.Maps;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Initialization for the Arena map.
/// </summary>
internal class Arena : Initialization.BaseMapInitializer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Arena"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Arena(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    protected override byte MapNumber => 6;

    /// <inheritdoc/>
    protected override string MapName => "Arena";

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateNpcSpawns()
    {
        yield return this.CreateMonsterSpawn(this.NpcDictionary[240], 58, 58, 140, 140, 1, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(this.NpcDictionary[200], 62, 62, 160, 160, 1, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(this.NpcDictionary[239], 67, 67, 140, 140, 1, Direction.SouthWest);
    }

    /// <inheritdoc/>
    protected override void AdditionalInitialization(GameMapDefinition mapDefinition)
    {
        var battleZone = this.Context.CreateNew<BattleZoneDefinition>();
        battleZone.Type = BattleType.Soccer;
        battleZone.LeftTeamSpawnPointX = 60;
        battleZone.LeftTeamSpawnPointY = 156;
        battleZone.RightTeamSpawnPointX = 60;
        battleZone.RightTeamSpawnPointY = 164;
        battleZone.Ground = this.CreateRectangle(55, 141, 69, 180);
        battleZone.LeftGoal = this.CreateRectangle(61, 139, 63, 140);
        battleZone.RightGoal = this.CreateRectangle(61, 181, 63, 182);
        mapDefinition.BattleZone = battleZone;
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        // no monsters to create
    }

    private Rectangle CreateRectangle(byte x1, byte y1, byte x2, byte y2)
    {
        var rectangle = this.Context.CreateNew<Rectangle>();
        rectangle.X1 = x1;
        rectangle.X2 = x2;
        rectangle.Y1 = y1;
        rectangle.Y2 = y2;
        return rectangle;
    }
}