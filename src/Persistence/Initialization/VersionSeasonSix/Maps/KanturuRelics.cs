// <copyright file="KanturuRelics.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps
{
    using System;
    using System.Collections.Generic;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.NPC;

    /// <summary>
    /// The initialization for the Kanturu Relics map.
    /// </summary>
    internal class KanturuRelics : BaseMapInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KanturuRelics"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public KanturuRelics(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc/>
        protected override byte MapNumber => 38;

        /// <inheritdoc/>
        protected override string MapName => "Kanturu_III"; // Kanturu Relics, Kanturu Remain

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateNpcSpawns()
        {
            yield return this.CreateMonsterSpawn(this.NpcDictionary[367], 141, 191, Direction.South); // Gateway Machine
        }

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
        {
            yield return this.CreateMonsterSpawn(this.NpcDictionary[358], 106, 159); // Persona
            yield return this.CreateMonsterSpawn(this.NpcDictionary[358], 140, 076); // Persona
            yield return this.CreateMonsterSpawn(this.NpcDictionary[358], 097, 078); // Persona
            yield return this.CreateMonsterSpawn(this.NpcDictionary[358], 124, 069); // Persona
            yield return this.CreateMonsterSpawn(this.NpcDictionary[358], 138, 067); // Persona
            yield return this.CreateMonsterSpawn(this.NpcDictionary[358], 183, 086); // Persona
            yield return this.CreateMonsterSpawn(this.NpcDictionary[358], 175, 104); // Persona
            yield return this.CreateMonsterSpawn(this.NpcDictionary[358], 111, 159); // Persona
            yield return this.CreateMonsterSpawn(this.NpcDictionary[358], 105, 133); // Persona
            yield return this.CreateMonsterSpawn(this.NpcDictionary[358], 119, 126); // Persona
            yield return this.CreateMonsterSpawn(this.NpcDictionary[358], 135, 150); // Persona
            yield return this.CreateMonsterSpawn(this.NpcDictionary[358], 177, 129); // Persona
            yield return this.CreateMonsterSpawn(this.NpcDictionary[358], 193, 146); // Persona
            yield return this.CreateMonsterSpawn(this.NpcDictionary[358], 195, 105); // Persona
            yield return this.CreateMonsterSpawn(this.NpcDictionary[358], 163, 082); // Persona
            yield return this.CreateMonsterSpawn(this.NpcDictionary[358], 132, 097); // Persona
            yield return this.CreateMonsterSpawn(this.NpcDictionary[358], 158, 137); // Persona
            yield return this.CreateMonsterSpawn(this.NpcDictionary[358], 171, 141); // Persona
            yield return this.CreateMonsterSpawn(this.NpcDictionary[358], 147, 148); // Persona
            yield return this.CreateMonsterSpawn(this.NpcDictionary[358], 152, 120); // Persona
            yield return this.CreateMonsterSpawn(this.NpcDictionary[358], 102, 078); // Persona
            yield return this.CreateMonsterSpawn(this.NpcDictionary[358], 128, 069); // Persona
            yield return this.CreateMonsterSpawn(this.NpcDictionary[358], 156, 120); // Persona
            yield return this.CreateMonsterSpawn(this.NpcDictionary[358], 127, 135); // Persona
            yield return this.CreateMonsterSpawn(this.NpcDictionary[358], 106, 154); // Persona
            yield return this.CreateMonsterSpawn(this.NpcDictionary[358], 111, 154); // Persona
            yield return this.CreateMonsterSpawn(this.NpcDictionary[358], 108, 148); // Persona

            yield return this.CreateMonsterSpawn(this.NpcDictionary[359], 159, 098); // Twin Tale
            yield return this.CreateMonsterSpawn(this.NpcDictionary[359], 100, 097); // Twin Tale
            yield return this.CreateMonsterSpawn(this.NpcDictionary[359], 099, 085); // Twin Tale
            yield return this.CreateMonsterSpawn(this.NpcDictionary[359], 125, 074); // Twin Tale
            yield return this.CreateMonsterSpawn(this.NpcDictionary[359], 113, 096); // Twin Tale
            yield return this.CreateMonsterSpawn(this.NpcDictionary[359], 111, 079); // Twin Tale
            yield return this.CreateMonsterSpawn(this.NpcDictionary[359], 123, 088); // Twin Tale
            yield return this.CreateMonsterSpawn(this.NpcDictionary[359], 139, 099); // Twin Tale
            yield return this.CreateMonsterSpawn(this.NpcDictionary[359], 178, 092); // Twin Tale
            yield return this.CreateMonsterSpawn(this.NpcDictionary[359], 198, 121); // Twin Tale
            yield return this.CreateMonsterSpawn(this.NpcDictionary[359], 197, 136); // Twin Tale
            yield return this.CreateMonsterSpawn(this.NpcDictionary[359], 126, 098); // Twin Tale
            yield return this.CreateMonsterSpawn(this.NpcDictionary[359], 135, 088); // Twin Tale
            yield return this.CreateMonsterSpawn(this.NpcDictionary[359], 117, 135); // Twin Tale
            yield return this.CreateMonsterSpawn(this.NpcDictionary[359], 101, 139); // Twin Tale
            yield return this.CreateMonsterSpawn(this.NpcDictionary[359], 149, 085); // Twin Tale
            yield return this.CreateMonsterSpawn(this.NpcDictionary[359], 153, 078); // Twin Tale
            yield return this.CreateMonsterSpawn(this.NpcDictionary[359], 151, 160); // Twin Tale
            yield return this.CreateMonsterSpawn(this.NpcDictionary[359], 179, 139); // Twin Tale
            yield return this.CreateMonsterSpawn(this.NpcDictionary[359], 187, 145); // Twin Tale
            yield return this.CreateMonsterSpawn(this.NpcDictionary[359], 168, 094); // Twin Tale
            yield return this.CreateMonsterSpawn(this.NpcDictionary[359], 199, 131); // Twin Tale
            yield return this.CreateMonsterSpawn(this.NpcDictionary[359], 195, 115); // Twin Tale
            yield return this.CreateMonsterSpawn(this.NpcDictionary[359], 185, 097); // Twin Tale

            yield return this.CreateMonsterSpawn(this.NpcDictionary[360], 110, 140); // Dreadfear
            yield return this.CreateMonsterSpawn(this.NpcDictionary[360], 122, 127); // Dreadfear
            yield return this.CreateMonsterSpawn(this.NpcDictionary[360], 140, 162); // Dreadfear
            yield return this.CreateMonsterSpawn(this.NpcDictionary[360], 176, 117); // Dreadfear
            yield return this.CreateMonsterSpawn(this.NpcDictionary[360], 162, 129); // Dreadfear
            yield return this.CreateMonsterSpawn(this.NpcDictionary[360], 167, 129); // Dreadfear
            yield return this.CreateMonsterSpawn(this.NpcDictionary[360], 137, 142); // Dreadfear
            yield return this.CreateMonsterSpawn(this.NpcDictionary[360], 150, 151); // Dreadfear
            yield return this.CreateMonsterSpawn(this.NpcDictionary[360], 163, 148); // Dreadfear
            yield return this.CreateMonsterSpawn(this.NpcDictionary[360], 177, 150); // Dreadfear
            yield return this.CreateMonsterSpawn(this.NpcDictionary[360], 152, 110); // Dreadfear
            yield return this.CreateMonsterSpawn(this.NpcDictionary[360], 164, 112); // Dreadfear

            // Trap:
            yield return this.CreateMonsterSpawn(this.NpcDictionary[105], 097, 096); // Canon Trap
            yield return this.CreateMonsterSpawn(this.NpcDictionary[105], 097, 078); // Canon Trap
            yield return this.CreateMonsterSpawn(this.NpcDictionary[105], 122, 074); // Canon Trap
            yield return this.CreateMonsterSpawn(this.NpcDictionary[105], 136, 070); // Canon Trap
            yield return this.CreateMonsterSpawn(this.NpcDictionary[105], 152, 078); // Canon Trap
            yield return this.CreateMonsterSpawn(this.NpcDictionary[105], 177, 089); // Canon Trap
            yield return this.CreateMonsterSpawn(this.NpcDictionary[105], 186, 105); // Canon Trap
            yield return this.CreateMonsterSpawn(this.NpcDictionary[105], 201, 134); // Canon Trap
            yield return this.CreateMonsterSpawn(this.NpcDictionary[105], 150, 152); // Canon Trap
            yield return this.CreateMonsterSpawn(this.NpcDictionary[105], 138, 152); // Canon Trap
            yield return this.CreateMonsterSpawn(this.NpcDictionary[105], 107, 153); // Canon Trap
            yield return this.CreateMonsterSpawn(this.NpcDictionary[105], 111, 153); // Canon Trap
            yield return this.CreateMonsterSpawn(this.NpcDictionary[105], 123, 079); // Canon Trap
            yield return this.CreateMonsterSpawn(this.NpcDictionary[105], 137, 104); // Canon Trap

            yield return this.CreateMonsterSpawn(this.NpcDictionary[106], 138, 166); // Laser Trap
            yield return this.CreateMonsterSpawn(this.NpcDictionary[106], 138, 171); // Laser Trap
            yield return this.CreateMonsterSpawn(this.NpcDictionary[106], 138, 177); // Laser Trap
            yield return this.CreateMonsterSpawn(this.NpcDictionary[106], 138, 183); // Laser Trap
            //// yield return this.CreateMonsterSpawn(context, mapDefinition, NpcDictionary[106], 1, Direction.Undefined, SpawnTrigger.Automatic, 141, 141, 155, 155); // Laser Trap
            yield return this.CreateMonsterSpawn(this.NpcDictionary[106], 141, 166); // Laser Trap
            yield return this.CreateMonsterSpawn(this.NpcDictionary[106], 141, 171); // Laser Trap
            yield return this.CreateMonsterSpawn(this.NpcDictionary[106], 141, 177); // Laser Trap
            yield return this.CreateMonsterSpawn(this.NpcDictionary[106], 141, 183); // Laser Trap
            yield return this.CreateMonsterSpawn(this.NpcDictionary[106], 144, 166); // Laser Trap
            yield return this.CreateMonsterSpawn(this.NpcDictionary[106], 144, 171); // Laser Trap
            yield return this.CreateMonsterSpawn(this.NpcDictionary[106], 144, 177); // Laser Trap
            yield return this.CreateMonsterSpawn(this.NpcDictionary[106], 144, 183); // Laser Trap
        }

        /// <inheritdoc/>
        protected override void CreateMonsters()
        {
            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 105;
                monster.Designation = "Canon Trap";
                monster.MoveRange = 0;
                monster.AttackRange = 4;
                monster.ViewRange = 2;
                monster.ObjectKind = NpcObjectKind.Trap;
                monster.IntelligenceTypeName = typeof(RandomAttackInRangeTrapIntelligence).FullName;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1000 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(3 * TimeSpan.TicksPerSecond);
                monster.Attribute = 1;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 80 },
                    { Stats.MaximumHealth, 1000 },
                    { Stats.MinimumPhysBaseDmg, 200 },
                    { Stats.MaximumPhysBaseDmg, 203 },
                    { Stats.AttackRatePvm, 400 },
                    { Stats.DefenseRatePvm, 500 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 106;
                monster.Designation = "Laser Trap";
                monster.MoveRange = 0;
                monster.AttackRange = 1;
                monster.ViewRange = 1;
                monster.ObjectKind = NpcObjectKind.Trap;
                monster.IntelligenceTypeName = typeof(RandomAttackInRangeTrapIntelligence).FullName;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1000 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(3 * TimeSpan.TicksPerSecond);
                monster.Attribute = 1;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 80 },
                    { Stats.MaximumHealth, 1000 },
                    { Stats.MinimumPhysBaseDmg, 150 },
                    { Stats.MaximumPhysBaseDmg, 152 },
                    { Stats.AttackRatePvm, 400 },
                    { Stats.DefenseRatePvm, 500 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 358;
                monster.Designation = "Persona";
                monster.MoveRange = 3;
                monster.AttackRange = 5;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 118 },
                    { Stats.MaximumHealth, 68000 },
                    { Stats.MinimumPhysBaseDmg, 1168 },
                    { Stats.MaximumPhysBaseDmg, 1213 },
                    { Stats.DefenseBase, 615 },
                    { Stats.AttackRatePvm, 1190 },
                    { Stats.DefenseRatePvm, 485 },
                    { Stats.PoisonResistance, 29f / 255 },
                    { Stats.IceResistance, 29f / 255 },
                    { Stats.LightningResistance, 29f / 255 },
                    { Stats.FireResistance, 29f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 359;
                monster.Designation = "Twin Tale";
                monster.MoveRange = 3;
                monster.AttackRange = 4;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 117 },
                    { Stats.MaximumHealth, 87500 },
                    { Stats.MinimumPhysBaseDmg, 830 },
                    { Stats.MaximumPhysBaseDmg, 1085 },
                    { Stats.DefenseBase, 865 },
                    { Stats.AttackRatePvm, 1080 },
                    { Stats.DefenseRatePvm, 440 },
                    { Stats.PoisonResistance, 28f / 255 },
                    { Stats.IceResistance, 28f / 255 },
                    { Stats.LightningResistance, 28f / 255 },
                    { Stats.FireResistance, 28f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 360;
                monster.Designation = "Dreadfear";
                monster.MoveRange = 3;
                monster.AttackRange = 2;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 119 },
                    { Stats.MaximumHealth, 94000 },
                    { Stats.MinimumPhysBaseDmg, 946 },
                    { Stats.MaximumPhysBaseDmg, 996 },
                    { Stats.DefenseBase, 783 },
                    { Stats.AttackRatePvm, 1015 },
                    { Stats.DefenseRatePvm, 906 },
                    { Stats.PoisonResistance, 27f / 255 },
                    { Stats.IceResistance, 27f / 255 },
                    { Stats.LightningResistance, 27f / 255 },
                    { Stats.FireResistance, 27f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }
        }
    }
}
