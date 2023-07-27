// <copyright file="DevilSquare2.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version095d.Maps;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Version095d.Events;

/// <summary>
/// Initialization for the devil square map which hosts devil square 1 to 4.
/// </summary>
internal class DevilSquare2 : BaseMapInitializer
{
    /// <summary>
    /// The default number of the map.
    /// </summary>
    internal const byte Number = 9;

    /// <summary>
    /// The default name of the map.
    /// </summary>
    internal const string Name = "Devil Square 2";

    /// <summary>
    /// Initializes a new instance of the <see cref="DevilSquare2"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public DevilSquare2(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    protected override byte MapNumber => Number;

    /// <inheritdoc/>
    protected override string MapName => Name;

    /// <inheritdoc/>
    protected override byte Discriminator => 2;

    /// <inheritdoc/>
    protected override byte SafezoneMapNumber => Noria.Number;

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        const byte x1 = 121;
        const byte x2 = 151;
        const byte y1 = 152;
        const byte y2 = 184;
        const byte quantity = 35;

        yield return this.CreateMonsterSpawn(1, this.NpcDictionary[10], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.FirstWaveNumber); // Dark Knight
        yield return this.CreateMonsterSpawn(2, this.NpcDictionary[39], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.FirstWaveNumber); // Poison Shadow

        yield return this.CreateMonsterSpawn(3, this.NpcDictionary[34], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.SecondWaveNumber); // Cursed Wizard
        yield return this.CreateMonsterSpawn(4, this.NpcDictionary[41], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.SecondWaveNumber); // Death Cow

        yield return this.CreateMonsterSpawn(5, this.NpcDictionary[40], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.ThirdWaveNumber); // Death Knight
        yield return this.CreateMonsterSpawn(6, this.NpcDictionary[35], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.ThirdWaveNumber); // Death Gorgon

        yield return this.CreateMonsterSpawn(7, this.NpcDictionary[49], x1, x2, y1, y2, 5, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, DevilSquareInitializer.BossWaveNumber); // Hydra
    }
}