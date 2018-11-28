// <copyright file="KanturuRelics.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Maps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.Attributes;

    /// <summary>
    /// The initialization for the Kanturu Relics map.
    /// </summary>
    internal class KanturuRelics : BaseMapInitializer
    {
        /// <summary>
        /// The default number of the kanturu relics map.
        /// </summary>
        public static readonly byte Number = 38;

        /// <inheritdoc/>
        protected override byte MapNumber => Number;

        /// <inheritdoc/>
        protected override string MapName => "Kanturu_III"; // Kanturu Relics, Kanturu Remain

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // NPCs:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[367], 1, Direction.South, SpawnTrigger.Automatic, 141, 141, 191, 191); // Gateway Machine

            // Monsters:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[358], 1, Direction.Undefined, SpawnTrigger.Automatic, 106, 106, 159, 159); // Persona
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[358], 1, Direction.Undefined, SpawnTrigger.Automatic, 140, 140, 076, 076); // Persona
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[358], 1, Direction.Undefined, SpawnTrigger.Automatic, 097, 097, 078, 078); // Persona
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[358], 1, Direction.Undefined, SpawnTrigger.Automatic, 124, 124, 069, 069); // Persona
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[358], 1, Direction.Undefined, SpawnTrigger.Automatic, 138, 138, 067, 067); // Persona
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[358], 1, Direction.Undefined, SpawnTrigger.Automatic, 183, 183, 086, 086); // Persona
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[358], 1, Direction.Undefined, SpawnTrigger.Automatic, 175, 175, 104, 104); // Persona
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[358], 1, Direction.Undefined, SpawnTrigger.Automatic, 111, 111, 159, 159); // Persona
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[358], 1, Direction.Undefined, SpawnTrigger.Automatic, 105, 105, 133, 133); // Persona
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[358], 1, Direction.Undefined, SpawnTrigger.Automatic, 119, 119, 126, 126); // Persona
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[358], 1, Direction.Undefined, SpawnTrigger.Automatic, 135, 135, 150, 150); // Persona
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[358], 1, Direction.Undefined, SpawnTrigger.Automatic, 177, 177, 129, 129); // Persona
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[358], 1, Direction.Undefined, SpawnTrigger.Automatic, 193, 193, 146, 146); // Persona
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[358], 1, Direction.Undefined, SpawnTrigger.Automatic, 195, 195, 105, 105); // Persona
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[358], 1, Direction.Undefined, SpawnTrigger.Automatic, 163, 163, 082, 082); // Persona
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[358], 1, Direction.Undefined, SpawnTrigger.Automatic, 132, 132, 097, 097); // Persona
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[358], 1, Direction.Undefined, SpawnTrigger.Automatic, 158, 158, 137, 137); // Persona
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[358], 1, Direction.Undefined, SpawnTrigger.Automatic, 171, 171, 141, 141); // Persona
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[358], 1, Direction.Undefined, SpawnTrigger.Automatic, 147, 147, 148, 148); // Persona
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[358], 1, Direction.Undefined, SpawnTrigger.Automatic, 152, 152, 120, 120); // Persona
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[358], 1, Direction.Undefined, SpawnTrigger.Automatic, 102, 102, 078, 078); // Persona
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[358], 1, Direction.Undefined, SpawnTrigger.Automatic, 128, 128, 069, 069); // Persona
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[358], 1, Direction.Undefined, SpawnTrigger.Automatic, 156, 156, 120, 120); // Persona
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[358], 1, Direction.Undefined, SpawnTrigger.Automatic, 127, 127, 135, 135); // Persona
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[358], 1, Direction.Undefined, SpawnTrigger.Automatic, 106, 106, 154, 154); // Persona
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[358], 1, Direction.Undefined, SpawnTrigger.Automatic, 111, 111, 154, 154); // Persona
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[358], 1, Direction.Undefined, SpawnTrigger.Automatic, 108, 108, 148, 148); // Persona

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[359], 1, Direction.Undefined, SpawnTrigger.Automatic, 159, 159, 098, 098); // Twin Tale
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[359], 1, Direction.Undefined, SpawnTrigger.Automatic, 100, 100, 097, 097); // Twin Tale
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[359], 1, Direction.Undefined, SpawnTrigger.Automatic, 099, 099, 085, 085); // Twin Tale
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[359], 1, Direction.Undefined, SpawnTrigger.Automatic, 125, 125, 074, 074); // Twin Tale
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[359], 1, Direction.Undefined, SpawnTrigger.Automatic, 113, 113, 096, 096); // Twin Tale
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[359], 1, Direction.Undefined, SpawnTrigger.Automatic, 111, 111, 079, 079); // Twin Tale
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[359], 1, Direction.Undefined, SpawnTrigger.Automatic, 123, 123, 088, 088); // Twin Tale
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[359], 1, Direction.Undefined, SpawnTrigger.Automatic, 139, 139, 099, 099); // Twin Tale
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[359], 1, Direction.Undefined, SpawnTrigger.Automatic, 178, 178, 092, 092); // Twin Tale
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[359], 1, Direction.Undefined, SpawnTrigger.Automatic, 198, 198, 121, 121); // Twin Tale
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[359], 1, Direction.Undefined, SpawnTrigger.Automatic, 197, 197, 136, 136); // Twin Tale
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[359], 1, Direction.Undefined, SpawnTrigger.Automatic, 126, 126, 098, 098); // Twin Tale
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[359], 1, Direction.Undefined, SpawnTrigger.Automatic, 135, 135, 088, 088); // Twin Tale
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[359], 1, Direction.Undefined, SpawnTrigger.Automatic, 117, 117, 135, 135); // Twin Tale
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[359], 1, Direction.Undefined, SpawnTrigger.Automatic, 101, 101, 139, 139); // Twin Tale
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[359], 1, Direction.Undefined, SpawnTrigger.Automatic, 149, 149, 085, 085); // Twin Tale
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[359], 1, Direction.Undefined, SpawnTrigger.Automatic, 153, 153, 078, 078); // Twin Tale
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[359], 1, Direction.Undefined, SpawnTrigger.Automatic, 151, 151, 160, 160); // Twin Tale
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[359], 1, Direction.Undefined, SpawnTrigger.Automatic, 179, 179, 139, 139); // Twin Tale
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[359], 1, Direction.Undefined, SpawnTrigger.Automatic, 187, 187, 145, 145); // Twin Tale
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[359], 1, Direction.Undefined, SpawnTrigger.Automatic, 168, 168, 094, 094); // Twin Tale
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[359], 1, Direction.Undefined, SpawnTrigger.Automatic, 199, 199, 131, 131); // Twin Tale
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[359], 1, Direction.Undefined, SpawnTrigger.Automatic, 195, 195, 115, 115); // Twin Tale
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[359], 1, Direction.Undefined, SpawnTrigger.Automatic, 185, 185, 097, 097); // Twin Tale

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[360], 1, Direction.Undefined, SpawnTrigger.Automatic, 110, 110, 140, 140); // Dreadfear
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[360], 1, Direction.Undefined, SpawnTrigger.Automatic, 122, 122, 127, 127); // Dreadfear
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[360], 1, Direction.Undefined, SpawnTrigger.Automatic, 140, 140, 162, 162); // Dreadfear
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[360], 1, Direction.Undefined, SpawnTrigger.Automatic, 176, 176, 117, 117); // Dreadfear
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[360], 1, Direction.Undefined, SpawnTrigger.Automatic, 162, 162, 129, 129); // Dreadfear
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[360], 1, Direction.Undefined, SpawnTrigger.Automatic, 167, 167, 129, 129); // Dreadfear
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[360], 1, Direction.Undefined, SpawnTrigger.Automatic, 137, 137, 142, 142); // Dreadfear
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[360], 1, Direction.Undefined, SpawnTrigger.Automatic, 150, 150, 151, 151); // Dreadfear
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[360], 1, Direction.Undefined, SpawnTrigger.Automatic, 163, 163, 148, 148); // Dreadfear
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[360], 1, Direction.Undefined, SpawnTrigger.Automatic, 177, 177, 150, 150); // Dreadfear
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[360], 1, Direction.Undefined, SpawnTrigger.Automatic, 152, 152, 110, 110); // Dreadfear
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[360], 1, Direction.Undefined, SpawnTrigger.Automatic, 164, 164, 112, 112); // Dreadfear

            // Trap:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[105], 1, Direction.Undefined, SpawnTrigger.Automatic, 097, 097, 096, 096); // Canon Trap
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[105], 1, Direction.Undefined, SpawnTrigger.Automatic, 097, 097, 078, 078); // Canon Trap
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[105], 1, Direction.Undefined, SpawnTrigger.Automatic, 122, 122, 074, 074); // Canon Trap
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[105], 1, Direction.Undefined, SpawnTrigger.Automatic, 136, 136, 070, 070); // Canon Trap
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[105], 1, Direction.Undefined, SpawnTrigger.Automatic, 152, 152, 078, 078); // Canon Trap
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[105], 1, Direction.Undefined, SpawnTrigger.Automatic, 177, 177, 089, 089); // Canon Trap
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[105], 1, Direction.Undefined, SpawnTrigger.Automatic, 186, 186, 105, 105); // Canon Trap
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[105], 1, Direction.Undefined, SpawnTrigger.Automatic, 201, 201, 134, 134); // Canon Trap
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[105], 1, Direction.Undefined, SpawnTrigger.Automatic, 150, 150, 152, 152); // Canon Trap
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[105], 1, Direction.Undefined, SpawnTrigger.Automatic, 138, 138, 152, 152); // Canon Trap
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[105], 1, Direction.Undefined, SpawnTrigger.Automatic, 107, 107, 153, 153); // Canon Trap
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[105], 1, Direction.Undefined, SpawnTrigger.Automatic, 111, 111, 153, 153); // Canon Trap
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[105], 1, Direction.Undefined, SpawnTrigger.Automatic, 123, 123, 079, 079); // Canon Trap
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[105], 1, Direction.Undefined, SpawnTrigger.Automatic, 137, 137, 104, 104); // Canon Trap

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[106], 1, Direction.Undefined, SpawnTrigger.Automatic, 138, 138, 166, 166); // Laser Trap
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[106], 1, Direction.Undefined, SpawnTrigger.Automatic, 138, 138, 171, 171); // Laser Trap
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[106], 1, Direction.Undefined, SpawnTrigger.Automatic, 138, 138, 177, 177); // Laser Trap
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[106], 1, Direction.Undefined, SpawnTrigger.Automatic, 138, 138, 183, 183); // Laser Trap
            //// yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[106], 1, Direction.Undefined, SpawnTrigger.Automatic, 141, 141, 155, 155); // Laser Trap
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[106], 1, Direction.Undefined, SpawnTrigger.Automatic, 141, 141, 166, 166); // Laser Trap
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[106], 1, Direction.Undefined, SpawnTrigger.Automatic, 141, 141, 171, 171); // Laser Trap
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[106], 1, Direction.Undefined, SpawnTrigger.Automatic, 141, 141, 177, 177); // Laser Trap
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[106], 1, Direction.Undefined, SpawnTrigger.Automatic, 141, 141, 183, 183); // Laser Trap
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[106], 1, Direction.Undefined, SpawnTrigger.Automatic, 144, 144, 166, 166); // Laser Trap
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[106], 1, Direction.Undefined, SpawnTrigger.Automatic, 144, 144, 171, 171); // Laser Trap
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[106], 1, Direction.Undefined, SpawnTrigger.Automatic, 144, 144, 177, 177); // Laser Trap
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[106], 1, Direction.Undefined, SpawnTrigger.Automatic, 144, 144, 183, 183); // Laser Trap
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 105;
                monster.Designation = "Canon Trap";
                monster.MoveRange = 0;
                monster.AttackRange = 4;
                monster.ViewRange = 2;
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

                monster.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 106;
                monster.Designation = "Laser Trap";
                monster.MoveRange = 0;
                monster.AttackRange = 1;
                monster.ViewRange = 1;
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

                monster.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
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
                    { Stats.PoisonResistance, 29 },
                    { Stats.IceResistance, 29 },
                    { Stats.LightningResistance, 29 },
                    { Stats.FireResistance, 29 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
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
                    { Stats.PoisonResistance, 28 },
                    { Stats.IceResistance, 28 },
                    { Stats.LightningResistance, 28 },
                    { Stats.FireResistance, 28 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
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
                    { Stats.PoisonResistance, 27 },
                    { Stats.IceResistance, 27 },
                    { Stats.LightningResistance, 27 },
                    { Stats.FireResistance, 27 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }
        }
    }
}
