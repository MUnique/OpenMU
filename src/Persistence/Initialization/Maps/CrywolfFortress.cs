// <copyright file="CrywolfFortress.cs" company="MUnique">
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
    /// The initialization for the Crywolf Fortress map.
    /// </summary>
    internal class CrywolfFortress : BaseMapInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CrywolfFortress"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public CrywolfFortress(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc/>
        protected override byte MapNumber => 34;

        /// <inheritdoc/>
        protected override string MapName => "Crywolf Fortress";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns()
        {
            var npcDictionary = this.GameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // NPCs:
            yield return this.CreateMonsterSpawn(npcDictionary[406], 228, 048, Direction.SouthWest); // Apostle Devin
            yield return this.CreateMonsterSpawn(npcDictionary[407], 062, 239, Direction.SouthWest); // Werewolf Quarel
            yield return this.CreateMonsterSpawn(npcDictionary[226], 135, 047, Direction.SouthWest); // Treiner
            yield return this.CreateMonsterSpawn(npcDictionary[248], 099, 040, Direction.SouthEast); // Wandering Merchant
            yield return this.CreateMonsterSpawn(npcDictionary[256], 096, 025, Direction.SouthEast); // Lahap
            yield return this.CreateMonsterSpawn(npcDictionary[251], 145, 014, Direction.SouthEast); // Hanzo the Blacksmith
            yield return this.CreateMonsterSpawn(npcDictionary[240], 113, 056, Direction.SouthWest); // Baz The Vault Keeper
            yield return this.CreateMonsterSpawn(npcDictionary[224], 118, 011, Direction.SouthEast); // Guardsman

            yield return this.CreateMonsterSpawn(npcDictionary[204], 121, 031, Direction.South); // Wolf Status
            yield return this.CreateMonsterSpawn(npcDictionary[205], 125, 027, Direction.South); // Wolf Altar1
            yield return this.CreateMonsterSpawn(npcDictionary[206], 126, 035, Direction.South); // Wolf Altar2
            yield return this.CreateMonsterSpawn(npcDictionary[207], 120, 038, Direction.South); // Wolf Altar3
            yield return this.CreateMonsterSpawn(npcDictionary[208], 115, 035, Direction.South); // Wolf Altar4
            yield return this.CreateMonsterSpawn(npcDictionary[209], 117, 027, Direction.South); // Wolf Altar5

            // Monsters:
            yield return this.CreateMonsterSpawn(npcDictionary[310], 058, 047); // Hammer Scout
            yield return this.CreateMonsterSpawn(npcDictionary[310], 060, 038); // Hammer Scout
            yield return this.CreateMonsterSpawn(npcDictionary[310], 065, 024); // Hammer Scout
            yield return this.CreateMonsterSpawn(npcDictionary[310], 070, 037); // Hammer Scout
            yield return this.CreateMonsterSpawn(npcDictionary[310], 074, 020); // Hammer Scout
            yield return this.CreateMonsterSpawn(npcDictionary[310], 077, 032); // Hammer Scout
            yield return this.CreateMonsterSpawn(npcDictionary[310], 078, 051); // Hammer Scout
            yield return this.CreateMonsterSpawn(npcDictionary[310], 079, 043); // Hammer Scout
            yield return this.CreateMonsterSpawn(npcDictionary[310], 083, 010); // Hammer Scout
            yield return this.CreateMonsterSpawn(npcDictionary[310], 088, 050); // Hammer Scout
            yield return this.CreateMonsterSpawn(npcDictionary[310], 088, 060); // Hammer Scout
            yield return this.CreateMonsterSpawn(npcDictionary[310], 094, 054); // Hammer Scout
            yield return this.CreateMonsterSpawn(npcDictionary[310], 098, 060); // Hammer Scout
            yield return this.CreateMonsterSpawn(npcDictionary[310], 150, 060); // Hammer Scout
            yield return this.CreateMonsterSpawn(npcDictionary[310], 159, 060); // Hammer Scout
            yield return this.CreateMonsterSpawn(npcDictionary[310], 162, 041); // Hammer Scout
            yield return this.CreateMonsterSpawn(npcDictionary[310], 164, 018); // Hammer Scout
            yield return this.CreateMonsterSpawn(npcDictionary[310], 169, 060); // Hammer Scout
            yield return this.CreateMonsterSpawn(npcDictionary[310], 176, 032); // Hammer Scout
            yield return this.CreateMonsterSpawn(npcDictionary[310], 178, 022); // Hammer Scout
            yield return this.CreateMonsterSpawn(npcDictionary[310], 179, 048); // Hammer Scout
            yield return this.CreateMonsterSpawn(npcDictionary[310], 185, 035); // Hammer Scout
            yield return this.CreateMonsterSpawn(npcDictionary[310], 191, 027); // Hammer Scout
            yield return this.CreateMonsterSpawn(npcDictionary[310], 194, 044); // Hammer Scout
            yield return this.CreateMonsterSpawn(npcDictionary[310], 195, 033); // Hammer Scout
            yield return this.CreateMonsterSpawn(npcDictionary[310], 199, 060); // Hammer Scout
            yield return this.CreateMonsterSpawn(npcDictionary[310], 201, 040); // Hammer Scout
            yield return this.CreateMonsterSpawn(npcDictionary[310], 205, 046); // Hammer Scout
            yield return this.CreateMonsterSpawn(npcDictionary[310], 206, 045); // Hammer Scout
            yield return this.CreateMonsterSpawn(npcDictionary[310], 214, 060); // Hammer Scout

            yield return this.CreateMonsterSpawn(npcDictionary[311], 022, 100); // Lance Scout
            yield return this.CreateMonsterSpawn(npcDictionary[311], 027, 122); // Lance Scout
            yield return this.CreateMonsterSpawn(npcDictionary[311], 035, 078); // Lance Scout
            yield return this.CreateMonsterSpawn(npcDictionary[311], 047, 053); // Lance Scout
            yield return this.CreateMonsterSpawn(npcDictionary[311], 047, 065); // Lance Scout
            yield return this.CreateMonsterSpawn(npcDictionary[311], 079, 078); // Lance Scout
            yield return this.CreateMonsterSpawn(npcDictionary[311], 082, 067); // Lance Scout
            yield return this.CreateMonsterSpawn(npcDictionary[311], 095, 078); // Lance Scout
            yield return this.CreateMonsterSpawn(npcDictionary[311], 103, 071); // Lance Scout
            yield return this.CreateMonsterSpawn(npcDictionary[311], 113, 078); // Lance Scout
            yield return this.CreateMonsterSpawn(npcDictionary[311], 117, 099); // Lance Scout
            yield return this.CreateMonsterSpawn(npcDictionary[311], 122, 078); // Lance Scout
            yield return this.CreateMonsterSpawn(npcDictionary[311], 135, 069); // Lance Scout
            yield return this.CreateMonsterSpawn(npcDictionary[311], 138, 078); // Lance Scout
            yield return this.CreateMonsterSpawn(npcDictionary[311], 146, 068); // Lance Scout
            yield return this.CreateMonsterSpawn(npcDictionary[311], 156, 069); // Lance Scout
            yield return this.CreateMonsterSpawn(npcDictionary[311], 157, 078); // Lance Scout
            yield return this.CreateMonsterSpawn(npcDictionary[311], 159, 049); // Lance Scout
            yield return this.CreateMonsterSpawn(npcDictionary[311], 170, 047); // Lance Scout
            yield return this.CreateMonsterSpawn(npcDictionary[311], 170, 078); // Lance Scout
            yield return this.CreateMonsterSpawn(npcDictionary[311], 173, 097); // Lance Scout
            yield return this.CreateMonsterSpawn(npcDictionary[311], 174, 054); // Lance Scout
            yield return this.CreateMonsterSpawn(npcDictionary[311], 177, 078); // Lance Scout
            yield return this.CreateMonsterSpawn(npcDictionary[311], 197, 078); // Lance Scout
            yield return this.CreateMonsterSpawn(npcDictionary[311], 210, 088); // Lance Scout
            yield return this.CreateMonsterSpawn(npcDictionary[311], 211, 069); // Lance Scout
            yield return this.CreateMonsterSpawn(npcDictionary[311], 221, 078); // Lance Scout
            yield return this.CreateMonsterSpawn(npcDictionary[311], 223, 120); // Lance Scout
            yield return this.CreateMonsterSpawn(npcDictionary[311], 225, 068); // Lance Scout
            yield return this.CreateMonsterSpawn(npcDictionary[311], 231, 078); // Lance Scout

            yield return this.CreateMonsterSpawn(npcDictionary[312], 021, 088); // Bow Scout
            yield return this.CreateMonsterSpawn(npcDictionary[312], 023, 115); // Bow Scout
            yield return this.CreateMonsterSpawn(npcDictionary[312], 030, 088); // Bow Scout
            yield return this.CreateMonsterSpawn(npcDictionary[312], 056, 115); // Bow Scout
            yield return this.CreateMonsterSpawn(npcDictionary[312], 068, 115); // Bow Scout
            yield return this.CreateMonsterSpawn(npcDictionary[312], 078, 088); // Bow Scout
            yield return this.CreateMonsterSpawn(npcDictionary[312], 079, 115); // Bow Scout
            yield return this.CreateMonsterSpawn(npcDictionary[312], 083, 101); // Bow Scout
            yield return this.CreateMonsterSpawn(npcDictionary[312], 090, 088); // Bow Scout
            yield return this.CreateMonsterSpawn(npcDictionary[312], 100, 088); // Bow Scout
            yield return this.CreateMonsterSpawn(npcDictionary[312], 114, 088); // Bow Scout
            yield return this.CreateMonsterSpawn(npcDictionary[312], 122, 115); // Bow Scout
            yield return this.CreateMonsterSpawn(npcDictionary[312], 124, 088); // Bow Scout
            yield return this.CreateMonsterSpawn(npcDictionary[312], 135, 088); // Bow Scout
            yield return this.CreateMonsterSpawn(npcDictionary[312], 145, 099); // Bow Scout
            yield return this.CreateMonsterSpawn(npcDictionary[312], 146, 088); // Bow Scout
            yield return this.CreateMonsterSpawn(npcDictionary[312], 155, 115); // Bow Scout
            yield return this.CreateMonsterSpawn(npcDictionary[312], 156, 084); // Bow Scout
            yield return this.CreateMonsterSpawn(npcDictionary[312], 168, 088); // Bow Scout
            yield return this.CreateMonsterSpawn(npcDictionary[312], 178, 088); // Bow Scout
            yield return this.CreateMonsterSpawn(npcDictionary[312], 192, 088); // Bow Scout
            yield return this.CreateMonsterSpawn(npcDictionary[312], 201, 089); // Bow Scout
            yield return this.CreateMonsterSpawn(npcDictionary[312], 202, 098); // Bow Scout
            yield return this.CreateMonsterSpawn(npcDictionary[312], 217, 094); // Bow Scout
            yield return this.CreateMonsterSpawn(npcDictionary[312], 233, 102); // Bow Scout
            yield return this.CreateMonsterSpawn(npcDictionary[312], 236, 088); // Bow Scout

            yield return this.CreateMonsterSpawn(npcDictionary[313], 019, 131); // Werewolf
            yield return this.CreateMonsterSpawn(npcDictionary[313], 020, 145); // Werewolf
            yield return this.CreateMonsterSpawn(npcDictionary[313], 033, 130); // Werewolf
            yield return this.CreateMonsterSpawn(npcDictionary[313], 046, 123); // Werewolf
            yield return this.CreateMonsterSpawn(npcDictionary[313], 071, 109); // Werewolf
            yield return this.CreateMonsterSpawn(npcDictionary[313], 076, 145); // Werewolf
            yield return this.CreateMonsterSpawn(npcDictionary[313], 082, 107); // Werewolf
            yield return this.CreateMonsterSpawn(npcDictionary[313], 082, 130); // Werewolf
            yield return this.CreateMonsterSpawn(npcDictionary[313], 088, 188, 144, 144); // Werewolf
            yield return this.CreateMonsterSpawn(npcDictionary[313], 119, 108); // Werewolf
            yield return this.CreateMonsterSpawn(npcDictionary[313], 123, 130); // Werewolf
            yield return this.CreateMonsterSpawn(npcDictionary[313], 123, 145); // Werewolf
            yield return this.CreateMonsterSpawn(npcDictionary[313], 149, 110); // Werewolf
            yield return this.CreateMonsterSpawn(npcDictionary[313], 155, 098); // Werewolf
            yield return this.CreateMonsterSpawn(npcDictionary[313], 163, 130); // Werewolf
            yield return this.CreateMonsterSpawn(npcDictionary[313], 164, 144); // Werewolf
            yield return this.CreateMonsterSpawn(npcDictionary[313], 197, 145); // Werewolf
            yield return this.CreateMonsterSpawn(npcDictionary[313], 204, 130); // Werewolf
            yield return this.CreateMonsterSpawn(npcDictionary[313], 206, 110); // Werewolf
            yield return this.CreateMonsterSpawn(npcDictionary[313], 218, 110); // Werewolf
            yield return this.CreateMonsterSpawn(npcDictionary[313], 226, 130); // Werewolf
            yield return this.CreateMonsterSpawn(npcDictionary[313], 235, 110); // Werewolf
            yield return this.CreateMonsterSpawn(npcDictionary[313], 237, 144); // Werewolf
            yield return this.CreateMonsterSpawn(npcDictionary[313], 240, 131); // Werewolf

            yield return this.CreateMonsterSpawn(npcDictionary[314], 016, 156); // Scout(Hero)
            yield return this.CreateMonsterSpawn(npcDictionary[314], 028, 156); // Scout(Hero)
            yield return this.CreateMonsterSpawn(npcDictionary[314], 065, 148); // Scout(Hero)
            yield return this.CreateMonsterSpawn(npcDictionary[314], 092, 156); // Scout(Hero)
            yield return this.CreateMonsterSpawn(npcDictionary[314], 119, 156); // Scout(Hero)
            yield return this.CreateMonsterSpawn(npcDictionary[314], 159, 156); // Scout(Hero)
            yield return this.CreateMonsterSpawn(npcDictionary[314], 171, 148); // Scout(Hero)
            yield return this.CreateMonsterSpawn(npcDictionary[314], 177, 156); // Scout(Hero)
            yield return this.CreateMonsterSpawn(npcDictionary[314], 236, 156); // Scout(Hero)
            yield return this.CreateMonsterSpawn(npcDictionary[314], 236, 156); // Scout(Hero)

            yield return this.CreateMonsterSpawn(npcDictionary[315], 030, 172); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(npcDictionary[315], 049, 194); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(npcDictionary[315], 051, 213); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(npcDictionary[315], 052, 171); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(npcDictionary[315], 059, 211); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(npcDictionary[315], 068, 224); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(npcDictionary[315], 073, 210); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(npcDictionary[315], 085, 230); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(npcDictionary[315], 092, 219); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(npcDictionary[315], 100, 182); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(npcDictionary[315], 102, 171); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(npcDictionary[315], 115, 204); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(npcDictionary[315], 117, 193); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(npcDictionary[315], 133, 195); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(npcDictionary[315], 133, 214); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(npcDictionary[315], 145, 208); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(npcDictionary[315], 148, 195); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(npcDictionary[315], 159, 206); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(npcDictionary[315], 170, 208); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(npcDictionary[315], 175, 197); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(npcDictionary[315], 199, 200); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(npcDictionary[315], 222, 189); // Werewolf(Hero)

            yield return this.CreateMonsterSpawn(npcDictionary[316], 047, 184); // Balram
            yield return this.CreateMonsterSpawn(npcDictionary[316], 084, 201); // Balram
            yield return this.CreateMonsterSpawn(npcDictionary[316], 104, 194); // Balram
            yield return this.CreateMonsterSpawn(npcDictionary[316], 117, 184); // Balram
            yield return this.CreateMonsterSpawn(npcDictionary[316], 146, 184); // Balram
            yield return this.CreateMonsterSpawn(npcDictionary[316], 161, 194); // Balram
            yield return this.CreateMonsterSpawn(npcDictionary[316], 173, 184); // Balram
            yield return this.CreateMonsterSpawn(npcDictionary[316], 212, 193); // Balram
            yield return this.CreateMonsterSpawn(npcDictionary[316], 228, 180); // Balram
            yield return this.CreateMonsterSpawn(npcDictionary[316], 054, 217); // Balram
            yield return this.CreateMonsterSpawn(npcDictionary[316], 066, 217); // Balram
            yield return this.CreateMonsterSpawn(npcDictionary[316], 083, 217); // Balram
            yield return this.CreateMonsterSpawn(npcDictionary[316], 100, 217); // Balram
            yield return this.CreateMonsterSpawn(npcDictionary[316], 134, 215); // Balram
            yield return this.CreateMonsterSpawn(npcDictionary[316], 153, 215); // Balram

            yield return this.CreateMonsterSpawn(npcDictionary[317], 014, 164); // Soram
            yield return this.CreateMonsterSpawn(npcDictionary[317], 025, 164); // Soram
            yield return this.CreateMonsterSpawn(npcDictionary[317], 041, 171); // Soram
            yield return this.CreateMonsterSpawn(npcDictionary[317], 056, 164); // Soram
            yield return this.CreateMonsterSpawn(npcDictionary[317], 065, 234); // Soram
            yield return this.CreateMonsterSpawn(npcDictionary[317], 096, 164); // Soram
            yield return this.CreateMonsterSpawn(npcDictionary[317], 114, 164); // Soram
            yield return this.CreateMonsterSpawn(npcDictionary[317], 116, 172); // Soram
            yield return this.CreateMonsterSpawn(npcDictionary[317], 140, 230); // Soram
            yield return this.CreateMonsterSpawn(npcDictionary[317], 145, 174); // Soram
            yield return this.CreateMonsterSpawn(npcDictionary[317], 147, 164); // Soram
            yield return this.CreateMonsterSpawn(npcDictionary[317], 168, 229); // Soram
            yield return this.CreateMonsterSpawn(npcDictionary[317], 177, 167); // Soram
            yield return this.CreateMonsterSpawn(npcDictionary[317], 233, 169); // Soram
            yield return this.CreateMonsterSpawn(npcDictionary[317], 115, 221); // Soram
            yield return this.CreateMonsterSpawn(npcDictionary[317], 103, 206); // Soram
        }

        /// <inheritdoc/>
        protected override void CreateMonsters()
        {
            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 204;
                monster.Designation = "Wolf Status";
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 2 },
                    { Stats.MaximumHealth, 50 },
                    { Stats.MinimumPhysBaseDmg, 15 },
                    { Stats.MaximumPhysBaseDmg, 30 },
                    { Stats.DefenseBase, 70 },
                    { Stats.AttackRatePvm, 10 },
                    { Stats.DefenseRatePvm, 30 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 205;
                monster.Designation = "Wolf Altar1";
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 2 },
                    { Stats.MaximumHealth, 50 },
                    { Stats.MinimumPhysBaseDmg, 15 },
                    { Stats.MaximumPhysBaseDmg, 30 },
                    { Stats.DefenseBase, 70 },
                    { Stats.AttackRatePvm, 10 },
                    { Stats.DefenseRatePvm, 30 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 206;
                monster.Designation = "Wolf Altar2";
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 2 },
                    { Stats.MaximumHealth, 50 },
                    { Stats.MinimumPhysBaseDmg, 15 },
                    { Stats.MaximumPhysBaseDmg, 30 },
                    { Stats.DefenseBase, 70 },
                    { Stats.AttackRatePvm, 10 },
                    { Stats.DefenseRatePvm, 30 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 207;
                monster.Designation = "Wolf Altar3";
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 2 },
                    { Stats.MaximumHealth, 50 },
                    { Stats.MinimumPhysBaseDmg, 15 },
                    { Stats.MaximumPhysBaseDmg, 30 },
                    { Stats.DefenseBase, 70 },
                    { Stats.AttackRatePvm, 10 },
                    { Stats.DefenseRatePvm, 30 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 208;
                monster.Designation = "Wolf Altar4";
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 2 },
                    { Stats.MaximumHealth, 50 },
                    { Stats.MinimumPhysBaseDmg, 15 },
                    { Stats.MaximumPhysBaseDmg, 30 },
                    { Stats.DefenseBase, 70 },
                    { Stats.AttackRatePvm, 10 },
                    { Stats.DefenseRatePvm, 30 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 209;
                monster.Designation = "Wolf Altar5";
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 2 },
                    { Stats.MaximumHealth, 50 },
                    { Stats.MinimumPhysBaseDmg, 15 },
                    { Stats.MaximumPhysBaseDmg, 30 },
                    { Stats.DefenseBase, 70 },
                    { Stats.AttackRatePvm, 10 },
                    { Stats.DefenseRatePvm, 30 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 310;
                monster.Designation = "Hammer Scout";
                monster.MoveRange = 6;
                monster.AttackRange = 1;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(600 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 97 },
                    { Stats.MaximumHealth, 45000 },
                    { Stats.MinimumPhysBaseDmg, 530 },
                    { Stats.MaximumPhysBaseDmg, 560 },
                    { Stats.DefenseBase, 420 },
                    { Stats.AttackRatePvm, 670 },
                    { Stats.DefenseRatePvm, 250 },
                    { Stats.PoisonResistance, 15 },
                    { Stats.IceResistance, 15 },
                    { Stats.FireResistance, 15 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 311;
                monster.Designation = "Lance Scout";
                monster.MoveRange = 6;
                monster.AttackRange = 3;
                monster.ViewRange = 6;
                monster.MoveDelay = new TimeSpan(600 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 97 },
                    { Stats.MaximumHealth, 45000 },
                    { Stats.MinimumPhysBaseDmg, 530 },
                    { Stats.MaximumPhysBaseDmg, 560 },
                    { Stats.DefenseBase, 420 },
                    { Stats.AttackRatePvm, 670 },
                    { Stats.DefenseRatePvm, 250 },
                    { Stats.PoisonResistance, 17 },
                    { Stats.IceResistance, 17 },
                    { Stats.FireResistance, 17 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 312;
                monster.Designation = "Bow Scout";
                monster.MoveRange = 6;
                monster.AttackRange = 6;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 97 },
                    { Stats.MaximumHealth, 45000 },
                    { Stats.MinimumPhysBaseDmg, 530 },
                    { Stats.MaximumPhysBaseDmg, 560 },
                    { Stats.DefenseBase, 420 },
                    { Stats.AttackRatePvm, 670 },
                    { Stats.DefenseRatePvm, 250 },
                    { Stats.PoisonResistance, 19 },
                    { Stats.IceResistance, 19 },
                    { Stats.FireResistance, 19 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 313;
                monster.Designation = "Werewolf";
                monster.MoveRange = 6;
                monster.AttackRange = 1;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(600 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 118 },
                    { Stats.MaximumHealth, 110000 },
                    { Stats.MinimumPhysBaseDmg, 830 },
                    { Stats.MaximumPhysBaseDmg, 850 },
                    { Stats.DefenseBase, 680 },
                    { Stats.AttackRatePvm, 950 },
                    { Stats.DefenseRatePvm, 355 },
                    { Stats.PoisonResistance, 21 },
                    { Stats.IceResistance, 21 },
                    { Stats.FireResistance, 21 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 314;
                monster.Designation = "Scout(Hero)";
                monster.MoveRange = 6;
                monster.AttackRange = 2;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 123 },
                    { Stats.MaximumHealth, 120000 },
                    { Stats.MinimumPhysBaseDmg, 890 },
                    { Stats.MaximumPhysBaseDmg, 910 },
                    { Stats.DefenseBase, 740 },
                    { Stats.AttackRatePvm, 980 },
                    { Stats.DefenseRatePvm, 370 },
                    { Stats.PoisonResistance, 23 },
                    { Stats.IceResistance, 23 },
                    { Stats.FireResistance, 23 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 315;
                monster.Designation = "Werewolf(Hero)";
                monster.MoveRange = 6;
                monster.AttackRange = 2;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 127 },
                    { Stats.MaximumHealth, 123000 },
                    { Stats.MinimumPhysBaseDmg, 964 },
                    { Stats.MaximumPhysBaseDmg, 1015 },
                    { Stats.DefenseBase, 800 },
                    { Stats.AttackRatePvm, 1027 },
                    { Stats.DefenseRatePvm, 397 },
                    { Stats.PoisonResistance, 25 },
                    { Stats.IceResistance, 25 },
                    { Stats.FireResistance, 25 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 316;
                monster.Designation = "Balram";
                monster.MoveRange = 6;
                monster.AttackRange = 3;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 132 },
                    { Stats.MaximumHealth, 140000 },
                    { Stats.MinimumPhysBaseDmg, 1075 },
                    { Stats.MaximumPhysBaseDmg, 1140 },
                    { Stats.DefenseBase, 885 },
                    { Stats.AttackRatePvm, 1100 },
                    { Stats.DefenseRatePvm, 440 },
                    { Stats.PoisonResistance, 27 },
                    { Stats.IceResistance, 27 },
                    { Stats.FireResistance, 27 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 317;
                monster.Designation = "Soram";
                monster.MoveRange = 6;
                monster.AttackRange = 7;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 134 },
                    { Stats.MaximumHealth, 164000 },
                    { Stats.MinimumPhysBaseDmg, 1200 },
                    { Stats.MaximumPhysBaseDmg, 1300 },
                    { Stats.DefenseBase, 982 },
                    { Stats.AttackRatePvm, 1173 },
                    { Stats.DefenseRatePvm, 500 },
                    { Stats.PoisonResistance, 29 },
                    { Stats.IceResistance, 29 },
                    { Stats.FireResistance, 29 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }
        }
    }
}
