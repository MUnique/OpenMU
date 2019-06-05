﻿// <copyright file="Karutan1.cs" company="MUnique">
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
    /// The initialization for the Karutan1 map.
    /// </summary>
    internal class Karutan1 : BaseMapInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Karutan1"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public Karutan1(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc/>
        protected override byte MapNumber => 80;

        /// <inheritdoc/>
        protected override string MapName => "Karutan1";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns()
        {
            var npcDictionary = this.GameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // NPCs:
            yield return this.CreateMonsterSpawn(npcDictionary[577], 121, 102, Direction.SouthEast); // Leina the General Goods Merchant
            yield return this.CreateMonsterSpawn(npcDictionary[578], 117, 126, Direction.SouthEast); // Weapons Merchant Bolo
            yield return this.CreateMonsterSpawn(npcDictionary[240], 123, 132, Direction.South); // Safety Guardian
            yield return this.CreateMonsterSpawn(npcDictionary[240], 122, 096, Direction.SouthEast); // Safety Guardian
            yield return this.CreateMonsterSpawn(npcDictionary[240], 158, 126, Direction.SouthWest); // Safety Guardian

            // Monsters:
            yield return this.CreateMonsterSpawn(npcDictionary[569], 129, 059); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[569], 127, 066); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[569], 136, 065); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[569], 139, 073); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[569], 148, 081); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[569], 166, 066); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[569], 183, 043); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[569], 194, 042); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[569], 189, 072); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[569], 179, 089); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[569], 192, 109); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[569], 183, 122); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[569], 207, 110); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[569], 213, 097); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[569], 209, 089); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[569], 185, 100); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[569], 175, 116); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[569], 152, 070); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[569], 214, 034); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[569], 226, 103); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[569], 191, 157); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[569], 172, 156); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[569], 209, 170); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[569], 163, 178); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[569], 131, 164); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[569], 112, 184); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[569], 099, 177); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[569], 093, 185); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[569], 079, 175); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[569], 062, 123); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[569], 091, 072); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[569], 089, 067); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[569], 072, 131); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[569], 089, 118); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[569], 108, 193); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 132, 067); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 167, 048); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 158, 075); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 174, 057); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 180, 047); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 191, 035); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 207, 038); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 201, 031); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 198, 063); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 188, 063); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 184, 114); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 204, 204, 104, 114); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 201, 094); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 226, 111); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 183, 162); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 176, 181); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 146, 165); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 102, 188); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 098, 188); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 093, 169); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 109, 176); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 139, 156); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 161, 174); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 161, 159); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 210, 188); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 219, 172); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 230, 162); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 223, 162); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 225, 168); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 182, 175); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 193, 192); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 165, 092); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 173, 086); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 120, 180); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 127, 167); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 071, 126); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 066, 117); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 086, 086); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 092, 077); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 095, 069); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 088, 176); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 154, 167); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 163, 054); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 169, 070); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 170, 063); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 198, 100); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[570], 215, 103); // Bone Scorpion
            yield return this.CreateMonsterSpawn(npcDictionary[571], 181, 189); // Orcus
            yield return this.CreateMonsterSpawn(npcDictionary[571], 187, 181); // Orcus
            yield return this.CreateMonsterSpawn(npcDictionary[571], 202, 155); // Orcus
            yield return this.CreateMonsterSpawn(npcDictionary[571], 081, 127); // Orcus
            yield return this.CreateMonsterSpawn(npcDictionary[571], 093, 112); // Orcus
            yield return this.CreateMonsterSpawn(npcDictionary[571], 183, 053); // Orcus
            yield return this.CreateMonsterSpawn(npcDictionary[571], 206, 034); // Orcus
            yield return this.CreateMonsterSpawn(npcDictionary[571], 180, 102); // Orcus
            yield return this.CreateMonsterSpawn(npcDictionary[571], 200, 166); // Orcus
            yield return this.CreateMonsterSpawn(npcDictionary[571], 207, 182); // Orcus
            yield return this.CreateMonsterSpawn(npcDictionary[573], 039, 186); // Crypta
            yield return this.CreateMonsterSpawn(npcDictionary[573], 033, 162); // Crypta
            yield return this.CreateMonsterSpawn(npcDictionary[573], 036, 137); // Crypta
            yield return this.CreateMonsterSpawn(npcDictionary[573], 048, 153); // Crypta
            yield return this.CreateMonsterSpawn(npcDictionary[573], 063, 167); // Crypta
            yield return this.CreateMonsterSpawn(npcDictionary[573], 044, 090); // Crypta
            yield return this.CreateMonsterSpawn(npcDictionary[573], 035, 070); // Crypta
            yield return this.CreateMonsterSpawn(npcDictionary[573], 042, 056); // Crypta
            yield return this.CreateMonsterSpawn(npcDictionary[573], 051, 066); // Crypta
            yield return this.CreateMonsterSpawn(npcDictionary[573], 043, 083); // Crypta
            yield return this.CreateMonsterSpawn(npcDictionary[573], 058, 068); // Crypta
            yield return this.CreateMonsterSpawn(npcDictionary[573], 029, 144); // Crypta
            yield return this.CreateMonsterSpawn(npcDictionary[573], 041, 180); // Crypta
            yield return this.CreateMonsterSpawn(npcDictionary[573], 038, 167); // Crypta
            yield return this.CreateMonsterSpawn(npcDictionary[573], 042, 105); // Crypta
            yield return this.CreateMonsterSpawn(npcDictionary[573], 038, 132); // Crypta
            yield return this.CreateMonsterSpawn(npcDictionary[573], 044, 148); // Crypta
            yield return this.CreateMonsterSpawn(npcDictionary[573], 058, 178); // Crypta
            yield return this.CreateMonsterSpawn(npcDictionary[574], 040, 074); // Crypos
            yield return this.CreateMonsterSpawn(npcDictionary[574], 049, 058); // Crypos
            yield return this.CreateMonsterSpawn(npcDictionary[574], 036, 085); // Crypos
            yield return this.CreateMonsterSpawn(npcDictionary[574], 029, 151); // Crypos
            yield return this.CreateMonsterSpawn(npcDictionary[574], 038, 143); // Crypos
            yield return this.CreateMonsterSpawn(npcDictionary[574], 055, 160); // Crypos
            yield return this.CreateMonsterSpawn(npcDictionary[574], 029, 157); // Crypos
            yield return this.CreateMonsterSpawn(npcDictionary[574], 044, 160); // Crypos
            yield return this.CreateMonsterSpawn(npcDictionary[574], 038, 173); // Crypos
            yield return this.CreateMonsterSpawn(npcDictionary[574], 036, 088); // Crypos
            yield return this.CreateMonsterSpawn(npcDictionary[574], 053, 070); // Crypos
            yield return this.CreateMonsterSpawn(npcDictionary[574], 041, 096); // Crypos
            yield return this.CreateMonsterSpawn(npcDictionary[574], 032, 133); // Crypos
        }

        /// <inheritdoc/>
        protected override void CreateMonsters()
        {
            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 569;
                monster.Designation = "Venomous Chain Scorpion";
                monster.MoveRange = 6;
                monster.AttackRange = 2;
                monster.ViewRange = 10;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 99 },
                    { Stats.MaximumHealth, 50000 },
                    { Stats.MinimumPhysBaseDmg, 555 },
                    { Stats.MaximumPhysBaseDmg, 590 },
                    { Stats.DefenseBase, 445 },
                    { Stats.AttackRatePvm, 845 },
                    { Stats.DefenseRatePvm, 248 },
                    { Stats.PoisonResistance, 254 },
                    { Stats.IceResistance, 100 },
                    { Stats.FireResistance, 200 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 570;
                monster.Designation = "Bone Scorpion";
                monster.MoveRange = 6;
                monster.AttackRange = 2;
                monster.ViewRange = 10;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 103 },
                    { Stats.MaximumHealth, 60000 },
                    { Stats.MinimumPhysBaseDmg, 595 },
                    { Stats.MaximumPhysBaseDmg, 635 },
                    { Stats.DefenseBase, 283 },
                    { Stats.AttackRatePvm, 915 },
                    { Stats.DefenseRatePvm, 363 },
                    { Stats.PoisonResistance, 254 },
                    { Stats.IceResistance, 100 },
                    { Stats.FireResistance, 200 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 571;
                monster.Designation = "Orcus";
                monster.MoveRange = 6;
                monster.AttackRange = 2;
                monster.ViewRange = 10;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 105 },
                    { Stats.MaximumHealth, 65000 },
                    { Stats.MinimumPhysBaseDmg, 618 },
                    { Stats.MaximumPhysBaseDmg, 655 },
                    { Stats.DefenseBase, 518 },
                    { Stats.AttackRatePvm, 965 },
                    { Stats.DefenseRatePvm, 293 },
                    { Stats.PoisonResistance, 100 },
                    { Stats.IceResistance, 100 },
                    { Stats.FireResistance, 240 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 573;
                monster.Designation = "Crypta";
                monster.MoveRange = 6;
                monster.AttackRange = 1;
                monster.ViewRange = 10;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 111 },
                    { Stats.MaximumHealth, 78000 },
                    { Stats.MinimumPhysBaseDmg, 705 },
                    { Stats.MaximumPhysBaseDmg, 755 },
                    { Stats.DefenseBase, 560 },
                    { Stats.AttackRatePvm, 1080 },
                    { Stats.DefenseRatePvm, 340 },
                    { Stats.PoisonResistance, 254 },
                    { Stats.IceResistance, 150 },
                    { Stats.FireResistance, 150 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 574;
                monster.Designation = "Crypos";
                monster.MoveRange = 6;
                monster.AttackRange = 2;
                monster.ViewRange = 10;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 114 },
                    { Stats.MaximumHealth, 83000 },
                    { Stats.MinimumPhysBaseDmg, 720 },
                    { Stats.MaximumPhysBaseDmg, 770 },
                    { Stats.DefenseBase, 575 },
                    { Stats.AttackRatePvm, 11400 },
                    { Stats.DefenseRatePvm, 375 },
                    { Stats.PoisonResistance, 254 },
                    { Stats.IceResistance, 150 },
                    { Stats.FireResistance, 150 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }
        }
    }
}