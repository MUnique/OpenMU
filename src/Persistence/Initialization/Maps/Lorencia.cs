// <copyright file="Lorencia.cs" company="MUnique">
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
    /// The initialization for the Lorencia map.
    /// </summary>
    internal class Lorencia : BaseMapInitializer
    {
        /// <summary>
        /// The default number of the lorencia map.
        /// </summary>
        public static readonly byte Number = 0;

        /// <inheritdoc/>
        protected override byte MapNumber => Number;

        /// <inheritdoc/>
        protected override string MapName => "Lorencia";

        /// <inheritdoc />
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition map, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // NPCs:
            yield return this.CreateMonsterSpawn(context, map, npcDictionary[230], 1, Direction.SouthEast, SpawnTrigger.Automatic, 62, 62, 130, 130);
            yield return this.CreateMonsterSpawn(context, map, npcDictionary[248], 1, Direction.SouthEast, SpawnTrigger.Automatic, 6, 6, 145, 145);
            yield return this.CreateMonsterSpawn(context, map, npcDictionary[226], 1, Direction.SouthEast, SpawnTrigger.Automatic, 122, 122, 110, 110);
            yield return this.CreateMonsterSpawn(context, map, npcDictionary[236], 1, Direction.SouthEast, SpawnTrigger.Automatic, 175, 175, 120, 120);
            yield return this.CreateMonsterSpawn(context, map, npcDictionary[240], 1, Direction.SouthEast, SpawnTrigger.Automatic, 146, 146, 110, 110);
            yield return this.CreateMonsterSpawn(context, map, npcDictionary[240], 1, Direction.SouthWest, SpawnTrigger.Automatic, 147, 147, 145, 145);
            yield return this.CreateMonsterSpawn(context, map, npcDictionary[249], 1, Direction.SouthWest, SpawnTrigger.Automatic, 131, 131, 88, 88);
            yield return this.CreateMonsterSpawn(context, map, npcDictionary[249], 1, Direction.SouthEast, SpawnTrigger.Automatic, 173, 173, 125, 125);
            yield return this.CreateMonsterSpawn(context, map, npcDictionary[249], 1, Direction.NorthWest, SpawnTrigger.Automatic, 94, 94, 125, 125);
            yield return this.CreateMonsterSpawn(context, map, npcDictionary[249], 1, Direction.NorthWest, SpawnTrigger.Automatic, 94, 94, 130, 130);
            yield return this.CreateMonsterSpawn(context, map, npcDictionary[249], 1, Direction.SouthWest, SpawnTrigger.Automatic, 131, 131, 148, 148);
            yield return this.CreateMonsterSpawn(context, map, npcDictionary[247], 1, Direction.SouthEast, SpawnTrigger.Automatic, 114, 114, 125, 125);
            yield return this.CreateMonsterSpawn(context, map, npcDictionary[250], 1, Direction.South, SpawnTrigger.Automatic, 183, 183, 137, 137);
            yield return this.CreateMonsterSpawn(context, map, npcDictionary[251], 1, Direction.SouthEast, SpawnTrigger.Automatic, 116, 116, 141, 141);
            yield return this.CreateMonsterSpawn(context, map, npcDictionary[253], 1, Direction.South, SpawnTrigger.Automatic, 127, 127, 86, 86);
            yield return this.CreateMonsterSpawn(context, map, npcDictionary[254], 1, Direction.SouthEast, SpawnTrigger.Automatic, 118, 118, 113, 113);
            yield return this.CreateMonsterSpawn(context, map, npcDictionary[255], 1, Direction.SouthWest, SpawnTrigger.Automatic, 123, 123, 135, 135);
            yield return this.CreateMonsterSpawn(context, map, npcDictionary[257], 1, Direction.SouthWest, SpawnTrigger.Automatic, 96, 96, 129, 129);
            yield return this.CreateMonsterSpawn(context, map, npcDictionary[257], 1, Direction.SouthWest, SpawnTrigger.Automatic, 174, 174, 129, 129);
            yield return this.CreateMonsterSpawn(context, map, npcDictionary[257], 1, Direction.SouthEast, SpawnTrigger.Automatic, 130, 130, 128, 128);
            yield return this.CreateMonsterSpawn(context, map, npcDictionary[257], 1, Direction.SouthEast, SpawnTrigger.Automatic, 132, 132, 165, 165);
            yield return this.CreateMonsterSpawn(context, map, npcDictionary[229], 1, Direction.SouthWest, SpawnTrigger.Automatic, 136, 136, 88, 88);
            yield return this.CreateMonsterSpawn(context, map, npcDictionary[375], 1, Direction.SouthEast, SpawnTrigger.Automatic, 132, 132, 161, 161);
            yield return this.CreateMonsterSpawn(context, map, npcDictionary[543], 1, Direction.South, SpawnTrigger.Automatic, 141, 141, 143, 143);

            // Monsters:
            yield return this.CreateMonsterSpawn(context, map, npcDictionary[000], 45, Direction.Undefined, SpawnTrigger.Automatic, 135, 240, 020, 088);
            yield return this.CreateMonsterSpawn(context, map, npcDictionary[003], 45, Direction.Undefined, SpawnTrigger.Automatic, 180, 226, 090, 244);
            yield return this.CreateMonsterSpawn(context, map, npcDictionary[002], 40, Direction.Undefined, SpawnTrigger.Automatic, 180, 226, 090, 244);
            yield return this.CreateMonsterSpawn(context, map, npcDictionary[002], 20, Direction.Undefined, SpawnTrigger.Automatic, 135, 240, 020, 088);
            yield return this.CreateMonsterSpawn(context, map, npcDictionary[006], 20, Direction.Undefined, SpawnTrigger.Automatic, 095, 175, 168, 244);
            yield return this.CreateMonsterSpawn(context, map, npcDictionary[014], 15, Direction.Undefined, SpawnTrigger.Automatic, 095, 175, 168, 244);
            yield return this.CreateMonsterSpawn(context, map, npcDictionary[001], 45, Direction.Undefined, SpawnTrigger.Automatic, 008, 094, 011, 244);
            yield return this.CreateMonsterSpawn(context, map, npcDictionary[004], 45, Direction.Undefined, SpawnTrigger.Automatic, 008, 094, 011, 244);
            yield return this.CreateMonsterSpawn(context, map, npcDictionary[007], 15, Direction.Undefined, SpawnTrigger.Automatic, 008, 060, 011, 080);
        }

        /// <inheritdoc />
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            var bullFighter = context.CreateNew<MonsterDefinition>();
            gameConfiguration.Monsters.Add(bullFighter);
            bullFighter.Number = 0;
            bullFighter.Designation = "Bull Fighter";
            bullFighter.MoveRange = 3;
            bullFighter.AttackRange = 1;
            bullFighter.ViewRange = 5;
            bullFighter.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            bullFighter.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
            bullFighter.RespawnDelay = new TimeSpan(3 * TimeSpan.TicksPerSecond);
            bullFighter.Attribute = 2;
            bullFighter.NumberOfMaximumItemDrops = 1;
            new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 6 },
                { Stats.MaximumHealth, 100 },
                { Stats.MinimumPhysBaseDmg, 16 },
                { Stats.MaximumPhysBaseDmg, 20 },
                { Stats.DefenseBase, 6 },
                { Stats.AttackRatePvm, 28 },
                { Stats.DefenseRatePvm, 6 },
                { Stats.WindResistance, 0 },
                { Stats.PoisonResistance, 0 },
                { Stats.IceResistance, 0 },
                { Stats.WaterResistance, 0 },
                { Stats.FireResistance, 0 },
            }.Select(kvp =>
            {
                var attribute = context.CreateNew<MonsterAttribute>();
                attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                attribute.Value = kvp.Value;
                return attribute;
            }).ToList().ForEach(bullFighter.Attributes.Add);

            var hound = context.CreateNew<MonsterDefinition>();
            gameConfiguration.Monsters.Add(hound);
            hound.Number = 1;
            hound.Designation = "Hound";
            new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 9 },
                { Stats.MaximumHealth, 140 },
                { Stats.MinimumPhysBaseDmg, 22 },
                { Stats.MaximumPhysBaseDmg, 27 },
                { Stats.DefenseBase, 9 },
                { Stats.AttackRatePvm, 39 },
                { Stats.DefenseRatePvm, 9 },
                { Stats.WindResistance, 0 },
                { Stats.PoisonResistance, 0 },
                { Stats.IceResistance, 0 },
                { Stats.WaterResistance, 0 },
                { Stats.FireResistance, 0 },
            }.Select(kvp =>
            {
                var attribute = context.CreateNew<MonsterAttribute>();
                attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                attribute.Value = kvp.Value;
                return attribute;
            }).ToList().ForEach(hound.Attributes.Add);
            hound.MoveRange = 3;
            hound.AttackRange = 1;
            hound.ViewRange = 5;
            hound.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            hound.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
            hound.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            hound.Attribute = 2;
            hound.NumberOfMaximumItemDrops = 1;

            var budgeDragon = context.CreateNew<MonsterDefinition>();
            gameConfiguration.Monsters.Add(budgeDragon);
            budgeDragon.Number = 2;
            budgeDragon.Designation = "Budge Dragon";
            new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 4 },
                { Stats.MaximumHealth, 60 },
                { Stats.MinimumPhysBaseDmg, 10 },
                { Stats.MaximumPhysBaseDmg, 13 },
                { Stats.DefenseBase, 3 },
                { Stats.AttackRatePvm, 18 },
                { Stats.DefenseRatePvm, 3 },
                { Stats.WindResistance, 0 },
                { Stats.PoisonResistance, 0 },
                { Stats.IceResistance, 0 },
                { Stats.WaterResistance, 0 },
                { Stats.FireResistance, 0 },
            }.Select(kvp =>
            {
                var attribute = context.CreateNew<MonsterAttribute>();
                attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                attribute.Value = kvp.Value;
                return attribute;
            }).ToList().ForEach(budgeDragon.Attributes.Add);
            budgeDragon.MoveRange = 3;
            budgeDragon.AttackRange = 1;
            budgeDragon.ViewRange = 4;
            budgeDragon.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            budgeDragon.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
            budgeDragon.RespawnDelay = new TimeSpan(3 * TimeSpan.TicksPerSecond);
            budgeDragon.Attribute = 2;
            budgeDragon.NumberOfMaximumItemDrops = 1;

            var spider = context.CreateNew<MonsterDefinition>();
            gameConfiguration.Monsters.Add(spider);
            spider.Number = 3;
            spider.Designation = "Spider";
            new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 2 },
                { Stats.MaximumHealth, 30 },
                { Stats.MinimumPhysBaseDmg, 4 },
                { Stats.MaximumPhysBaseDmg, 7 },
                { Stats.DefenseBase, 1 },
                { Stats.AttackRatePvm, 8 },
                { Stats.DefenseRatePvm, 1 },
                { Stats.WindResistance, 0 },
                { Stats.PoisonResistance, 0 },
                { Stats.IceResistance, 0 },
                { Stats.WaterResistance, 0 },
                { Stats.FireResistance, 0 },
            }.Select(kvp =>
            {
                var attribute = context.CreateNew<MonsterAttribute>();
                attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                attribute.Value = kvp.Value;
                return attribute;
            }).ToList().ForEach(spider.Attributes.Add);
            spider.MoveRange = 2;
            spider.AttackRange = 1;
            spider.ViewRange = 5;
            spider.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            spider.AttackDelay = new TimeSpan(1800 * TimeSpan.TicksPerMillisecond);
            spider.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            spider.Attribute = 2;
            spider.NumberOfMaximumItemDrops = 1;

            var eliteBullFighter = context.CreateNew<MonsterDefinition>();
            gameConfiguration.Monsters.Add(eliteBullFighter);
            eliteBullFighter.Number = 4;
            eliteBullFighter.Designation = "Elite Bull Fighter";
            new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 12 },
                { Stats.MaximumHealth, 190 },
                { Stats.MinimumPhysBaseDmg, 31 },
                { Stats.MaximumPhysBaseDmg, 36 },
                { Stats.DefenseBase, 12 },
                { Stats.AttackRatePvm, 50 },
                { Stats.DefenseRatePvm, 12 },
                { Stats.WindResistance, 0 },
                { Stats.PoisonResistance, 0 },
                { Stats.IceResistance, 0 },
                { Stats.WaterResistance, 0 },
                { Stats.FireResistance, 0 },
            }.Select(kvp =>
            {
                var attribute = context.CreateNew<MonsterAttribute>();
                attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                attribute.Value = kvp.Value;
                return attribute;
            }).ToList().ForEach(eliteBullFighter.Attributes.Add);
            eliteBullFighter.MoveRange = 3;
            eliteBullFighter.AttackRange = 1;
            eliteBullFighter.ViewRange = 4;
            eliteBullFighter.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            eliteBullFighter.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
            eliteBullFighter.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            eliteBullFighter.Attribute = 2;
            eliteBullFighter.NumberOfMaximumItemDrops = 1;

            var lich = context.CreateNew<MonsterDefinition>();
            gameConfiguration.Monsters.Add(lich);
            lich.Number = 6;
            lich.Designation = "Lich";
            new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 14 },
                { Stats.MaximumHealth, 255 },
                { Stats.MinimumPhysBaseDmg, 41 },
                { Stats.MaximumPhysBaseDmg, 46 },
                { Stats.DefenseBase, 14 },
                { Stats.AttackRatePvm, 62 },
                { Stats.DefenseRatePvm, 14 },
                { Stats.WindResistance, 0 },
                { Stats.PoisonResistance, 0 },
                { Stats.IceResistance, 0 },
                { Stats.WaterResistance, 0 },
                { Stats.FireResistance, 1 },
            }.Select(kvp =>
            {
                var attribute = context.CreateNew<MonsterAttribute>();
                attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                attribute.Value = kvp.Value;
                return attribute;
            }).ToList().ForEach(lich.Attributes.Add);
            lich.MoveRange = 3;
            lich.AttackRange = 4;
            lich.ViewRange = 7;
            lich.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            lich.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
            lich.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            lich.Attribute = 2;
            lich.NumberOfMaximumItemDrops = 1;

            var giant = context.CreateNew<MonsterDefinition>();
            gameConfiguration.Monsters.Add(giant);
            giant.Number = 7;
            giant.Designation = "Giant";
            new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 17 },
                { Stats.MaximumHealth, 400 },
                { Stats.MinimumPhysBaseDmg, 57 },
                { Stats.MaximumPhysBaseDmg, 62 },
                { Stats.DefenseBase, 18 },
                { Stats.AttackRatePvm, 80 },
                { Stats.DefenseRatePvm, 18 },
                { Stats.WindResistance, 0 },
                { Stats.PoisonResistance, 0 },
                { Stats.IceResistance, 0 },
                { Stats.WaterResistance, 0 },
                { Stats.FireResistance, 0 },
            }.Select(kvp =>
            {
                var attribute = context.CreateNew<MonsterAttribute>();
                attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                attribute.Value = kvp.Value;
                return attribute;
            }).ToList().ForEach(giant.Attributes.Add);
            giant.MoveRange = 2;
            giant.AttackRange = 2;
            giant.ViewRange = 3;
            giant.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            giant.AttackDelay = new TimeSpan(2200 * TimeSpan.TicksPerMillisecond);
            giant.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            giant.Attribute = 2;
            giant.NumberOfMaximumItemDrops = 1;

            var skeleton = context.CreateNew<MonsterDefinition>();
            gameConfiguration.Monsters.Add(skeleton);
            skeleton.Number = 14;
            skeleton.Designation = "Skeleton Warrior";
            new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 19 },
                { Stats.MaximumHealth, 525 },
                { Stats.MinimumPhysBaseDmg, 68 },
                { Stats.MaximumPhysBaseDmg, 74 },
                { Stats.DefenseBase, 22 },
                { Stats.AttackRatePvm, 93 },
                { Stats.DefenseRatePvm, 22 },
                { Stats.WindResistance, 0 },
                { Stats.PoisonResistance, 0 },
                { Stats.IceResistance, 0 },
                { Stats.WaterResistance, 0 },
                { Stats.FireResistance, 0 },
            }.Select(kvp =>
            {
                var attribute = context.CreateNew<MonsterAttribute>();
                attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                attribute.Value = kvp.Value;
                return attribute;
            }).ToList().ForEach(skeleton.Attributes.Add);
            skeleton.MoveRange = 2;
            skeleton.AttackRange = 1;
            skeleton.ViewRange = 4;
            skeleton.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            skeleton.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
            skeleton.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            skeleton.Attribute = 2;
            skeleton.NumberOfMaximumItemDrops = 1;
        }
    }
}
