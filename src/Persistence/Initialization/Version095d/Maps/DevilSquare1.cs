// <copyright file="DevilSquare1.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version095d.Maps;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Version095d.Events;

/// <summary>
/// Initialization for the devil square map which hosts devil square 1.
/// </summary>
internal class DevilSquare1 : BaseMapInitializer
{
    /// <summary>
    /// The default number of the map.
    /// </summary>
    internal const byte Number = 9;

    /// <summary>
    /// The default name of the map.
    /// </summary>
    internal const string Name = "Devil Square 1";

    /// <summary>
    /// Initializes a new instance of the <see cref="DevilSquare1"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public DevilSquare1(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    protected override byte MapNumber => Number;

    /// <inheritdoc/>
    protected override string MapName => Name;

    /// <inheritdoc/>
    protected override byte Discriminator => 1;

    /// <inheritdoc/>
    protected override byte SafezoneMapNumber => Noria.Number;

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        const byte x1 = 119;
        const byte x2 = 150;
        const byte y1 = 80;
        const byte y2 = 115;
        const byte quantity = 35;

        yield return this.CreateMonsterSpawn(1, this.NpcDictionary[17], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.FirstWaveNumber); // Cyclop
        yield return this.CreateMonsterSpawn(2, this.NpcDictionary[15], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.FirstWaveNumber); // Skeleton Archer

        yield return this.CreateMonsterSpawn(3, this.NpcDictionary[5], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.SecondWaveNumber); // Hell Hound
        yield return this.CreateMonsterSpawn(4, this.NpcDictionary[13], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.SecondWaveNumber); // Hell Spider

        yield return this.CreateMonsterSpawn(5, this.NpcDictionary[8], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.ThirdWaveNumber); // Poison Bull
        yield return this.CreateMonsterSpawn(6, this.NpcDictionary[36], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.ThirdWaveNumber); // Shadow

        yield return this.CreateMonsterSpawn(7, this.NpcDictionary[18], x1, x2, y1, y2, 5, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, DevilSquareInitializer.BossWaveNumber); // Gorgon
    }
}