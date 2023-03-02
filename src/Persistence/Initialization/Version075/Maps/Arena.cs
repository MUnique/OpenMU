// <copyright file="Arena.cs" company="MUnique">
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
    /// The default number of the map.
    /// </summary>
    internal const byte Number = 6;

    /// <summary>
    /// The default name of the map.
    /// </summary>
    internal const string Name = "Arena";

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
    protected override byte MapNumber => Number;

    /// <inheritdoc/>
    protected override string MapName => Name;

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateNpcSpawns()
    {
        yield return this.CreateMonsterSpawn(1, this.NpcDictionary[240], 58, 58, 140, 140, 1, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(2, this.NpcDictionary[200], 62, 62, 160, 160, 1, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(3, this.NpcDictionary[239], 67, 67, 140, 140, 1, Direction.SouthWest);
    }

    /// <inheritdoc/>
    protected override void AdditionalInitialization(GameMapDefinition mapDefinition)
    {
        var battleZone = this.Context.CreateNew<BattleZoneDefinition>();
        battleZone.SetGuid(this.MapNumber, 1);
        battleZone.Type = BattleType.Soccer;
        battleZone.LeftTeamSpawnPointX = 60;
        battleZone.LeftTeamSpawnPointY = 156;
        battleZone.RightTeamSpawnPointX = 60;
        battleZone.RightTeamSpawnPointY = 164;
        battleZone.Ground = this.CreateRectangle(1, 55, 141, 69, 180);
        battleZone.LeftGoal = this.CreateRectangle(2, 61, 139, 63, 140);
        battleZone.RightGoal = this.CreateRectangle(3, 61, 181, 63, 182);
        mapDefinition.BattleZone = battleZone;
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        // no monsters to create
    }

    private Rectangle CreateRectangle(short number, byte x1, byte y1, byte x2, byte y2)
    {
        var rectangle = this.Context.CreateNew<Rectangle>();
        rectangle.SetGuid(this.MapNumber, number);
        rectangle.X1 = x1;
        rectangle.X2 = x2;
        rectangle.Y1 = y1;
        rectangle.Y2 = y2;
        return rectangle;
    }
}