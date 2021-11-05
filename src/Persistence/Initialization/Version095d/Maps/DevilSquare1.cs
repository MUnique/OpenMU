// <copyright file="DevilSquare1.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version095d.Maps
{
    using System.Collections.Generic;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.Persistence.Initialization.Version095d.Events;

    /// <summary>
    /// Initialization for the devil square map which hosts devil square 1.
    /// </summary>
    internal class DevilSquare1 : BaseMapInitializer
    {
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
        protected override byte MapNumber => 9;

        /// <inheritdoc/>
        protected override string MapName => "Devil Square 1";

        /// <inheritdoc/>
        protected override int Discriminator => 1;

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

            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.FirstWaveNumber); // Cyclop
            yield return this.CreateMonsterSpawn(this.NpcDictionary[15], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.FirstWaveNumber); // Skeleton Archer

            yield return this.CreateMonsterSpawn(this.NpcDictionary[5], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.SecondWaveNumber); // Hell Hound
            yield return this.CreateMonsterSpawn(this.NpcDictionary[13], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.SecondWaveNumber); // Hell Spider

            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.ThirdWaveNumber); // Poison Bull
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.ThirdWaveNumber); // Shadow

            yield return this.CreateMonsterSpawn(this.NpcDictionary[18], x1, x2, y1, y2, 5, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, DevilSquareInitializer.BossWaveNumber); // Gorgon
        }
    }
}
