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
    /// Intializes the lorencia map.
    /// </summary>
    public class Lorencia
    {
        /// <summary>
        /// Initializes the data for the lorencia map.
        /// </summary>
        /// <param name="repositoryManager">The repository manager.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        /// <returns>
        /// The created game map definition for lorencia.
        /// </returns>
        public GameMapDefinition Initialize(IRepositoryManager repositoryManager, GameConfiguration gameConfiguration)
        {
            this.CreateMonsters(repositoryManager, gameConfiguration);
            var mapDefinition = repositoryManager.CreateNew<GameMapDefinition>();
            mapDefinition.Number = 0;
            mapDefinition.Name = "Lorencia";
            mapDefinition.TerrainData = Terrains.ResourceManager.GetObject("Terrain1") as byte[];
            mapDefinition.ExpMultiplier = 1;
            foreach (var spawn in this.CreateSpawns(repositoryManager, mapDefinition, gameConfiguration))
            {
                mapDefinition.MonsterSpawns.Add(spawn);
            }

            return mapDefinition;
        }

        private MonsterSpawnArea CreateMonsterSpawn(IRepositoryManager repositoryManager, GameMapDefinition map, MonsterDefinition monsterDefinition, short quantity, byte direction, SpawnTrigger spawnTrigger, byte x1, byte x2, byte y1, byte y2)
        {
            var area = repositoryManager.CreateNew<MonsterSpawnArea>();
            area.GameMap = map;
            area.MonsterDefinition = monsterDefinition;
            area.Quantity = quantity;
            area.Direction = (Direction)direction;
            area.SpawnTrigger = spawnTrigger;
            area.X1 = x1;
            area.X2 = x2;
            area.Y1 = y1;
            area.Y2 = y2;
            return area;
        }

        /// <summary>
        /// Gets all lorencia npc spawns.
        /// </summary>
        /// <param name="repositoryManager">The repository manager.</param>
        /// <param name="map">The lorencia map.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        /// <returns>
        /// The spawn areas of lorencia.
        /// </returns>
        /// <remarks>
        /// Extracted from MonsterSetBase.txt by Regex: (?m)^(\d+)\t*?(\d+)\t*?(\d+)\t*?(\d+)\t*?(\d+)\t*?(\d+).*?$
        /// Replace by: yield return new MonsterSpawnArea { GameMap = maps.First\(m =&gt; m.Number == $2\), MonsterDefinition = npcDictionary\[$1\], Quantity = 1, Direction = $6, SpawnTrigger = SpawnTrigger.Automatic, X1 = $4, X2 = $4, Y1 = $5, Y2 = $5 };
        /// </remarks>
        private IEnumerable<MonsterSpawnArea> CreateSpawns(IRepositoryManager repositoryManager, GameMapDefinition map, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // NPCs:
            yield return this.CreateMonsterSpawn(repositoryManager, map, npcDictionary[230], 1, 3, SpawnTrigger.Automatic, 62, 62, 130, 130);
            yield return this.CreateMonsterSpawn(repositoryManager, map, npcDictionary[248], 1, 3, SpawnTrigger.Automatic, 6, 6, 145, 145);
            yield return this.CreateMonsterSpawn(repositoryManager, map, npcDictionary[226], 1, 3, SpawnTrigger.Automatic, 122, 122, 110, 110);
            yield return this.CreateMonsterSpawn(repositoryManager, map, npcDictionary[236], 1, 3, SpawnTrigger.Automatic, 175, 175, 120, 120);
            yield return this.CreateMonsterSpawn(repositoryManager, map, npcDictionary[240], 1, 3, SpawnTrigger.Automatic, 146, 146, 110, 110);
            yield return this.CreateMonsterSpawn(repositoryManager, map, npcDictionary[240], 1, 1, SpawnTrigger.Automatic, 147, 147, 145, 145);
            yield return this.CreateMonsterSpawn(repositoryManager, map, npcDictionary[249], 1, 1, SpawnTrigger.Automatic, 131, 131, 88, 88);
            yield return this.CreateMonsterSpawn(repositoryManager, map, npcDictionary[249], 1, 3, SpawnTrigger.Automatic, 173, 173, 125, 125);
            yield return this.CreateMonsterSpawn(repositoryManager, map, npcDictionary[249], 1, 7, SpawnTrigger.Automatic, 94, 94, 125, 125);
            yield return this.CreateMonsterSpawn(repositoryManager, map, npcDictionary[249], 1, 7, SpawnTrigger.Automatic, 94, 94, 130, 130);
            yield return this.CreateMonsterSpawn(repositoryManager, map, npcDictionary[249], 1, 1, SpawnTrigger.Automatic, 131, 131, 148, 148);
            yield return this.CreateMonsterSpawn(repositoryManager, map, npcDictionary[247], 1, 3, SpawnTrigger.Automatic, 114, 114, 125, 125);
            yield return this.CreateMonsterSpawn(repositoryManager, map, npcDictionary[250], 1, 2, SpawnTrigger.Automatic, 183, 183, 137, 137);
            yield return this.CreateMonsterSpawn(repositoryManager, map, npcDictionary[251], 1, 3, SpawnTrigger.Automatic, 116, 116, 141, 141);
            yield return this.CreateMonsterSpawn(repositoryManager, map, npcDictionary[253], 1, 2, SpawnTrigger.Automatic, 127, 127, 86, 86);
            yield return this.CreateMonsterSpawn(repositoryManager, map, npcDictionary[254], 1, 3, SpawnTrigger.Automatic, 118, 118, 113, 113);
            yield return this.CreateMonsterSpawn(repositoryManager, map, npcDictionary[255], 1, 1, SpawnTrigger.Automatic, 123, 123, 135, 135);
            yield return this.CreateMonsterSpawn(repositoryManager, map, npcDictionary[257], 1, 1, SpawnTrigger.Automatic, 96, 96, 129, 129);
            yield return this.CreateMonsterSpawn(repositoryManager, map, npcDictionary[257], 1, 1, SpawnTrigger.Automatic, 174, 174, 129, 129);
            yield return this.CreateMonsterSpawn(repositoryManager, map, npcDictionary[257], 1, 3, SpawnTrigger.Automatic, 130, 130, 128, 128);
            yield return this.CreateMonsterSpawn(repositoryManager, map, npcDictionary[257], 1, 3, SpawnTrigger.Automatic, 132, 132, 165, 165);
            yield return this.CreateMonsterSpawn(repositoryManager, map, npcDictionary[229], 1, 1, SpawnTrigger.Automatic, 136, 136, 88, 88);
            yield return this.CreateMonsterSpawn(repositoryManager, map, npcDictionary[375], 1, 3, SpawnTrigger.Automatic, 132, 132, 161, 161);

            // Monsters:
            yield return this.CreateMonsterSpawn(repositoryManager, map, npcDictionary[000], 45, 0, SpawnTrigger.Automatic, 135, 240, 020, 088);
            yield return this.CreateMonsterSpawn(repositoryManager, map, npcDictionary[003], 45, 0, SpawnTrigger.Automatic, 180, 226, 090, 244);
            yield return this.CreateMonsterSpawn(repositoryManager, map, npcDictionary[002], 40, 0, SpawnTrigger.Automatic, 180, 226, 090, 244);
            yield return this.CreateMonsterSpawn(repositoryManager, map, npcDictionary[002], 20, 0, SpawnTrigger.Automatic, 135, 240, 020, 088);
            yield return this.CreateMonsterSpawn(repositoryManager, map, npcDictionary[006], 20, 0, SpawnTrigger.Automatic, 095, 175, 168, 244);
            yield return this.CreateMonsterSpawn(repositoryManager, map, npcDictionary[014], 15, 0, SpawnTrigger.Automatic, 095, 175, 168, 244);
            yield return this.CreateMonsterSpawn(repositoryManager, map, npcDictionary[001], 45, 0, SpawnTrigger.Automatic, 008, 094, 011, 244);
            yield return this.CreateMonsterSpawn(repositoryManager, map, npcDictionary[004], 45, 0, SpawnTrigger.Automatic, 008, 094, 011, 244);
            yield return this.CreateMonsterSpawn(repositoryManager, map, npcDictionary[007], 15, 0, SpawnTrigger.Automatic, 008, 060, 011, 080);
        }

        /// <summary>
        /// Gets all lorencia monsters.
        /// </summary>
        /// <param name="repositoryManager">The repository manager.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        /// <remarks>
        /// Extracted from Monsters.txt by Regex: (?m)^(\d+)\t1\t"(.*?)"\t*?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+).*?$
        /// <![CDATA[Replace by: yield return new MonsterDefinition\r\n            {\r\n                Number = $1,\r\n                Designation = "$2",\r\n                Attributes = new Dictionary<AttributeDefinition, float>\r\n                {\r\n                    { Stats.Level, $3 },\r\n                    { Stats.MaximumHealth, $4 },\r\n                    { Stats.MinimumPhysBaseDmg, $6 },\r\n                    { Stats.MaximumPhysBaseDmg, $7 },\r\n                    { Stats.DefenseBase, $8 },\r\n                    { Stats.AttackRatePvm, $10 },\r\n                    { Stats.DefenseRatePvm, $11 },\r\n                    { Stats.WindResistance, $23 },\r\n                    { Stats.PoisonResistance, $24 },\r\n                    { Stats.IceResistance, $25 },\r\n                    { Stats.WaterResistance, $26 },\r\n                    { Stats.FireResistance, $27 },\r\n                },\r\n                MoveRange = $12,\r\n                AttackRange = $14,\r\n                ViewRange = $15,\r\n                MoveDelay = new TimeSpan\($16 * TimeSpan.TicksPerMillisecond\),\r\n                AttackDelay = new TimeSpan\($17 * TimeSpan.TicksPerMillisecond\),\r\n                RespawnDelay = new TimeSpan\($18 * TimeSpan.TicksPerSecond\),\r\n                Attribute = $19\r\n            };]]>
        /// </remarks>
        private void CreateMonsters(IRepositoryManager repositoryManager, GameConfiguration gameConfiguration)
        {
            var bullFighter = repositoryManager.CreateNew<MonsterDefinition>();
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
                var attribute = repositoryManager.CreateNew<MonsterAttribute>();
                attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                attribute.Value = kvp.Value;
                return attribute;
            }).ToList().ForEach(bullFighter.Attributes.Add);

            var hound = repositoryManager.CreateNew<MonsterDefinition>();
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
                var attribute = repositoryManager.CreateNew<MonsterAttribute>();
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

            var budgeDragon = repositoryManager.CreateNew<MonsterDefinition>();
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
                var attribute = repositoryManager.CreateNew<MonsterAttribute>();
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

            var spider = repositoryManager.CreateNew<MonsterDefinition>();
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
                var attribute = repositoryManager.CreateNew<MonsterAttribute>();
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

            var eliteBullFighter = repositoryManager.CreateNew<MonsterDefinition>();
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
                var attribute = repositoryManager.CreateNew<MonsterAttribute>();
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

            var lich = repositoryManager.CreateNew<MonsterDefinition>();
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
                var attribute = repositoryManager.CreateNew<MonsterAttribute>();
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

            var giant = repositoryManager.CreateNew<MonsterDefinition>();
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
                var attribute = repositoryManager.CreateNew<MonsterAttribute>();
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

            var skeleton = repositoryManager.CreateNew<MonsterDefinition>();
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
                var attribute = repositoryManager.CreateNew<MonsterAttribute>();
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
