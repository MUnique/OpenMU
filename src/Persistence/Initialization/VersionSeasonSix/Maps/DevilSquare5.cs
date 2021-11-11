﻿// <copyright file="DevilSquare5.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Events;

/// <summary>
/// Initialization for the devil square map which hosts devil square 5.
/// </summary>
internal class DevilSquare5 : BaseMapInitializer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DevilSquare5"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public DevilSquare5(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    protected override byte MapNumber => 32;

    /// <inheritdoc/>
    protected override string MapName => "Devil Square 5";

    /// <inheritdoc/>
    protected override int Discriminator => 5;

    /// <inheritdoc/>
    protected override byte SafezoneMapNumber => Noria.Number;

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        const byte x1 = 120;
        const byte x2 = 150;
        const byte y1 = 80;
        const byte y2 = 115;
        const byte quantity = 35;

        yield return this.CreateMonsterSpawn(this.NpcDictionary[294], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.FirstWaveNumber); // Axe Warrior
        yield return this.CreateMonsterSpawn(this.NpcDictionary[60], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.FirstWaveNumber); // Bloody Wolf

        yield return this.CreateMonsterSpawn(this.NpcDictionary[71], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.SecondWaveNumber); // Mega Crust
        yield return this.CreateMonsterSpawn(this.NpcDictionary[190], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.SecondWaveNumber); // Death Angel

        yield return this.CreateMonsterSpawn(this.NpcDictionary[61], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.ThirdWaveNumber); // Beam Knight
        yield return this.CreateMonsterSpawn(this.NpcDictionary[73], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.ThirdWaveNumber); // Drakan

        yield return this.CreateMonsterSpawn(this.NpcDictionary[291], x1, x2, y1, y2, 5, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, DevilSquareInitializer.BossWaveNumber); // Fire Golem
    }
}