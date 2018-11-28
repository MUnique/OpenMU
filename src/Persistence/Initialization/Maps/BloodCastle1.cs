// <copyright file="BloodCastle1.cs" company="MUnique">
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
    /// Initialization for the Blood Castle 1.
    /// </summary>
    internal class BloodCastle1 : BaseMapInitializer
    {
        /// <summary>
        /// The default number of the Blood Castle 1 map.
        /// </summary>
        public static readonly byte Number = 11;

        /// <inheritdoc/>
        protected override byte MapNumber => Number;

        /// <inheritdoc/>
        protected override string MapName => "Blood Castle 1";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // NPCs:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[131], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 075, 075); // Castle Gate
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[132], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 095, 095); // Statue of Saint
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[232], 1, Direction.SouthWest, SpawnTrigger.AutomaticDuringEvent, 010, 010, 009, 009); // Archangel

            // Monsters:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[084], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 034, 034); // Chief Skeleton Warrior 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[084], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 038, 038); // Chief Skeleton Warrior 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[084], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 035, 035); // Chief Skeleton Warrior 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[084], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 028, 028); // Chief Skeleton Warrior 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[084], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 035, 035); // Chief Skeleton Warrior 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[084], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 033, 033); // Chief Skeleton Warrior 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[084], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 041, 041); // Chief Skeleton Warrior 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[084], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 042, 042); // Chief Skeleton Warrior 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[084], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 025, 025); // Chief Skeleton Warrior 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[084], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 023, 023); // Chief Skeleton Warrior 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[084], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 026, 026); // Chief Skeleton Warrior 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[084], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 037, 037); // Chief Skeleton Warrior 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[084], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 040, 040); // Chief Skeleton Warrior 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[084], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 039, 039); // Chief Skeleton Warrior 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[084], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 027, 027); // Chief Skeleton Warrior 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[084], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 024, 024); // Chief Skeleton Warrior 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[084], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 032, 032); // Chief Skeleton Warrior 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[084], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 045, 045); // Chief Skeleton Warrior 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[084], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 031, 031); // Chief Skeleton Warrior 1

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[085], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 045, 045); // Chief Skeleton Archer 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[085], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 023, 023); // Chief Skeleton Archer 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[085], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 043, 043); // Chief Skeleton Archer 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[085], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 039, 039); // Chief Skeleton Archer 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[085], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 037, 037); // Chief Skeleton Archer 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[085], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 029, 029); // Chief Skeleton Archer 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[085], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 028, 028); // Chief Skeleton Archer 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[085], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 025, 025); // Chief Skeleton Archer 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[085], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 021, 021); // Chief Skeleton Archer 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[085], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 022, 022); // Chief Skeleton Archer 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[085], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 021, 021); // Chief Skeleton Archer 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[085], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 031, 031); // Chief Skeleton Archer 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[085], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 030, 030); // Chief Skeleton Archer 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[085], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 033, 033); // Chief Skeleton Archer 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[085], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 021, 021); // Chief Skeleton Archer 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[085], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 043, 043); // Chief Skeleton Archer 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[085], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 026, 026); // Chief Skeleton Archer 1

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[086], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 046, 046); // Dark Skull Soldier 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[086], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 063, 063); // Dark Skull Soldier 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[086], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 053, 053); // Dark Skull Soldier 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[086], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 065, 065); // Dark Skull Soldier 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[086], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 067, 067); // Dark Skull Soldier 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[086], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 059, 059); // Dark Skull Soldier 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[086], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 059, 059); // Dark Skull Soldier 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[086], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 055, 055); // Dark Skull Soldier 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[086], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 062, 062); // Dark Skull Soldier 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[086], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 041, 041); // Dark Skull Soldier 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[086], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 069, 069); // Dark Skull Soldier 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[086], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 050, 050); // Dark Skull Soldier 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[086], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 048, 048); // Dark Skull Soldier 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[086], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 046, 046); // Dark Skull Soldier 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[086], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 044, 044); // Dark Skull Soldier 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[086], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 051, 051); // Dark Skull Soldier 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[086], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 049, 049); // Dark Skull Soldier 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[086], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 057, 057); // Dark Skull Soldier 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[086], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 064, 064); // Dark Skull Soldier 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[086], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 067, 067); // Dark Skull Soldier 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[086], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 053, 053); // Dark Skull Soldier 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[086], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 065, 065); // Dark Skull Soldier 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[086], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 060, 060); // Dark Skull Soldier 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[086], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 057, 057); // Dark Skull Soldier 1

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[087], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 069, 069); // Giant Ogre 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[087], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 061, 061); // Giant Ogre 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[087], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 055, 055); // Giant Ogre 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[087], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 058, 058); // Giant Ogre 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[087], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 052, 052); // Giant Ogre 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[087], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 050, 050); // Giant Ogre 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[087], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 066, 066); // Giant Ogre 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[087], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 056, 056); // Giant Ogre 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[087], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 070, 070); // Giant Ogre 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[087], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 048, 048); // Giant Ogre 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[087], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 063, 063); // Giant Ogre 1

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[088], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 078, 078); // Red Skeleton Knight 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[088], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 083, 083); // Red Skeleton Knight 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[088], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 078, 078); // Red Skeleton Knight 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[088], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 078, 078); // Red Skeleton Knight 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[088], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 016, 016, 080, 080); // Red Skeleton Knight 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[088], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 062, 062); // Red Skeleton Knight 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[088], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 017, 017, 083, 083); // Red Skeleton Knight 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[088], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 090, 090); // Red Skeleton Knight 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[088], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 011, 011, 083, 083); // Red Skeleton Knight 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[088], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 094, 094); // Red Skeleton Knight 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[088], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 020, 020, 082, 082); // Red Skeleton Knight 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[088], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 020, 020, 088, 088); // Red Skeleton Knight 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[088], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 054, 054); // Red Skeleton Knight 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[088], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 036, 036); // Red Skeleton Knight 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[088], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 068, 068); // Red Skeleton Knight 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[088], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 047, 047); // Red Skeleton Knight 1

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[089], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 016, 016, 087, 087); // Magic Skeleton 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[089], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 012, 012, 081, 081); // Magic Skeleton 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[089], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 088, 088); // Magic Skeleton 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[089], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 085, 085); // Magic Skeleton 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[089], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 019, 019, 085, 085); // Magic Skeleton 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[089], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 093, 093); // Magic Skeleton 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[089], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 080, 080); // Magic Skeleton 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[089], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 018, 018, 081, 081); // Magic Skeleton 1
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 84;
                monster.Designation = "Chief Skeleton Warrior 1";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 3;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 56 },
                    { Stats.MaximumHealth, 5000 },
                    { Stats.MinimumPhysBaseDmg, 160 },
                    { Stats.MaximumPhysBaseDmg, 180 },
                    { Stats.DefenseBase, 110 },
                    { Stats.AttackRatePvm, 300 },
                    { Stats.DefenseRatePvm, 85 },
                    { Stats.PoisonResistance, 2 },
                    { Stats.IceResistance, 2 },
                    { Stats.FireResistance, 2 },
                    { Stats.LightningResistance, 2 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            } // 084 Chief Skeleton Warrior 1

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 85;
                monster.Designation = "Chief Skeleton Archer 1";
                monster.MoveRange = 3;
                monster.AttackRange = 5;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 61 },
                    { Stats.MaximumHealth, 6500 },
                    { Stats.MinimumPhysBaseDmg, 180 },
                    { Stats.MaximumPhysBaseDmg, 200 },
                    { Stats.DefenseBase, 120 },
                    { Stats.AttackRatePvm, 330 },
                    { Stats.DefenseRatePvm, 93 },
                    { Stats.PoisonResistance, 2 },
                    { Stats.IceResistance, 2 },
                    { Stats.FireResistance, 2 },
                    { Stats.LightningResistance, 2 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            } // 085 Chief Skeleton Archer 1

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 86;
                monster.Designation = "Dark Skull Soldier 1";
                monster.MoveRange = 3;
                monster.AttackRange = 4;
                monster.ViewRange = 3;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 66 },
                    { Stats.MaximumHealth, 8000 },
                    { Stats.MinimumPhysBaseDmg, 190 },
                    { Stats.MaximumPhysBaseDmg, 220 },
                    { Stats.DefenseBase, 160 },
                    { Stats.AttackRatePvm, 360 },
                    { Stats.DefenseRatePvm, 98 },
                    { Stats.PoisonResistance, 2 },
                    { Stats.IceResistance, 2 },
                    { Stats.FireResistance, 2 },
                    { Stats.LightningResistance, 2 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            } // 086 Dark Skull Soldier 1

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 87;
                monster.Designation = "Giant Ogre 1";
                monster.MoveRange = 3;
                monster.AttackRange = 5;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 70 },
                    { Stats.MaximumHealth, 9500 },
                    { Stats.MinimumPhysBaseDmg, 210 },
                    { Stats.MaximumPhysBaseDmg, 240 },
                    { Stats.DefenseBase, 180 },
                    { Stats.AttackRatePvm, 400 },
                    { Stats.DefenseRatePvm, 115 },
                    { Stats.PoisonResistance, 2 },
                    { Stats.IceResistance, 2 },
                    { Stats.FireResistance, 2 },
                    { Stats.LightningResistance, 2 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            } // 087 Giant Ogre 1

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 88;
                monster.Designation = "Red Skeleton Knight 1";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 3;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 74 },
                    { Stats.MaximumHealth, 12000 },
                    { Stats.MinimumPhysBaseDmg, 220 },
                    { Stats.MaximumPhysBaseDmg, 260 },
                    { Stats.DefenseBase, 190 },
                    { Stats.AttackRatePvm, 440 },
                    { Stats.DefenseRatePvm, 130 },
                    { Stats.PoisonResistance, 2 },
                    { Stats.IceResistance, 2 },
                    { Stats.FireResistance, 2 },
                    { Stats.LightningResistance, 2 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            } // 088 Red Skeleton Knight 1

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 89;
                monster.Designation = "Magic Skeleton 1";
                monster.MoveRange = 4;
                monster.AttackRange = 4;
                monster.ViewRange = 6;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1800 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 79 },
                    { Stats.MaximumHealth, 15000 },
                    { Stats.MinimumPhysBaseDmg, 230 },
                    { Stats.MaximumPhysBaseDmg, 280 },
                    { Stats.DefenseBase, 240 },
                    { Stats.AttackRatePvm, 500 },
                    { Stats.DefenseRatePvm, 180 },
                    { Stats.PoisonResistance, 5 },
                    { Stats.IceResistance, 5 },
                    { Stats.FireResistance, 5 },
                    { Stats.LightningResistance, 5 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            } // 089 Magic Skeleton 1
        }
    }
}
