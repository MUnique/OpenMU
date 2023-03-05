// <copyright file="DevilSquare6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Events;

/// <summary>
/// Initialization for the devil square map which hosts devil square 5.
/// </summary>
internal class DevilSquare6 : BaseMapInitializer
{
    /// <summary>
    /// The Number of the Map.
    /// </summary>
    internal const byte Number = 32;

    /// <summary>
    /// The Name of the Map.
    /// </summary>
    internal const string Name = "Devil Square 6";

    /// <summary>
    /// Initializes a new instance of the <see cref="DevilSquare6"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public DevilSquare6(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    protected override byte MapNumber => Number;

    /// <inheritdoc/>
    protected override string MapName => Name;

    /// <inheritdoc/>
    protected override byte Discriminator => 6;

    /// <inheritdoc/>
    protected override byte SafezoneMapNumber => Noria.Number;

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        const byte x1 = 122;
        const byte x2 = 151;
        const byte y1 = 152;
        const byte y2 = 184;
        const byte quantity = 35;

        yield return this.CreateMonsterSpawn(1, this.NpcDictionary[290], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.FirstWaveNumber); // Lizard Warrior
        yield return this.CreateMonsterSpawn(2, this.NpcDictionary[57], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.FirstWaveNumber); // Iron Wheel

        yield return this.CreateMonsterSpawn(3, this.NpcDictionary[70], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.SecondWaveNumber); // Queen Rainer
        yield return this.CreateMonsterSpawn(4, this.NpcDictionary[293], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.SecondWaveNumber); // Poison Golem

        yield return this.CreateMonsterSpawn(5, this.NpcDictionary[74], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.ThirdWaveNumber); // Alpha Crust
        yield return this.CreateMonsterSpawn(6, this.NpcDictionary[292], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.ThirdWaveNumber); // Queen Bee

        yield return this.CreateMonsterSpawn(7, this.NpcDictionary[197], x1, x2, y1, y2, 5, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, DevilSquareInitializer.BossWaveNumber); // Shadow of Kundun
    }
}