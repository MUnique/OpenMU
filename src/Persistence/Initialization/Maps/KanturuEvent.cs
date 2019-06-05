// <copyright file="KanturuEvent.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Maps
{
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.Attributes;

    /// <summary>
    /// Initialization for the Kanturu event map.
    /// </summary>
    internal class KanturuEvent : BaseMapInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KanturuEvent"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public KanturuEvent(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc/>
        protected override byte MapNumber => 39;

        /// <inheritdoc/>
        protected override string MapName => "Kanturu Event";

        /// <inheritdoc/>
        protected override void CreateMapAttributeRequirements()
        {
            // It's only required during the event. Events are not implemented yet - probably we need multiple GameMapDefinitions, for each state of a map.
            this.CreateRequirement(Stats.MoonstonePendantEquipped, 1);
        }

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns()
        {
            yield return this.CreateMonsterSpawn(this.GameConfiguration.Monsters.First(m => m.Number == 368), 77, 177, Direction.SouthWest); // Elpis NPC

            var laserTrap = this.GameConfiguration.Monsters.First(m => m.Number == 106);
            yield return this.CreateMonsterSpawn(laserTrap, 60, 108);
            yield return this.CreateMonsterSpawn(laserTrap, 173, 61);
            yield return this.CreateMonsterSpawn(laserTrap, 173, 64);
            yield return this.CreateMonsterSpawn(laserTrap, 173, 67);
            yield return this.CreateMonsterSpawn(laserTrap, 173, 70);
            yield return this.CreateMonsterSpawn(laserTrap, 173, 73);
            yield return this.CreateMonsterSpawn(laserTrap, 173, 76);
            yield return this.CreateMonsterSpawn(laserTrap, 173, 79);
            yield return this.CreateMonsterSpawn(laserTrap, 179, 89);
            yield return this.CreateMonsterSpawn(laserTrap, 176, 86);
            yield return this.CreateMonsterSpawn(laserTrap, 173, 82);
            yield return this.CreateMonsterSpawn(laserTrap, 201, 94);
            yield return this.CreateMonsterSpawn(laserTrap, 204, 92);
            yield return this.CreateMonsterSpawn(laserTrap, 207, 91);
            yield return this.CreateMonsterSpawn(laserTrap, 210, 89);
            yield return this.CreateMonsterSpawn(laserTrap, 212, 88);
            yield return this.CreateMonsterSpawn(laserTrap, 215, 86);
            yield return this.CreateMonsterSpawn(laserTrap, 217, 84);
            yield return this.CreateMonsterSpawn(laserTrap, 218, 81);
            yield return this.CreateMonsterSpawn(laserTrap, 218, 78);
            yield return this.CreateMonsterSpawn(laserTrap, 218, 73);
            yield return this.CreateMonsterSpawn(laserTrap, 218, 70);
            yield return this.CreateMonsterSpawn(laserTrap, 218, 67);
            yield return this.CreateMonsterSpawn(laserTrap, 218, 64);
            yield return this.CreateMonsterSpawn(laserTrap, 217, 60);
            yield return this.CreateMonsterSpawn(laserTrap, 214, 57);
            yield return this.CreateMonsterSpawn(laserTrap, 211, 54);
            yield return this.CreateMonsterSpawn(laserTrap, 208, 54);
            yield return this.CreateMonsterSpawn(laserTrap, 205, 54);
            yield return this.CreateMonsterSpawn(laserTrap, 201, 54);
            yield return this.CreateMonsterSpawn(laserTrap, 198, 54);
            yield return this.CreateMonsterSpawn(laserTrap, 193, 54);
            yield return this.CreateMonsterSpawn(laserTrap, 190, 54);
            yield return this.CreateMonsterSpawn(laserTrap, 185, 54);
            yield return this.CreateMonsterSpawn(laserTrap, 182, 54);
            yield return this.CreateMonsterSpawn(laserTrap, 178, 56);
            yield return this.CreateMonsterSpawn(laserTrap, 176, 58);
            yield return this.CreateMonsterSpawn(laserTrap, 174, 59);
        }

        /// <inheritdoc/>
        protected override void CreateMonsters()
        {
            // no monsters to create
        }
    }
}