// <copyright file="BaseMapInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Maps
{
    using System.Collections.Generic;
    using MUnique.OpenMU.DataModel.Configuration;

    /// <summary>
    /// Base class for a map initializer which provides some common basic functionality.
    /// </summary>
    internal abstract class BaseMapInitializer : IMapInitializer
    {
        /// <summary>
        /// Gets the map number which will be set as <see cref="GameMapDefinition.Number"/>.
        /// </summary>
        protected abstract byte MapNumber { get; }

        /// <summary>
        /// Gets the name of the map which will be set as <see cref="GameMapDefinition.Name"/>.
        /// </summary>
        protected abstract string MapName { get; }

        /// <inheritdoc />
        public GameMapDefinition Initialize(IContext context, GameConfiguration gameConfiguration)
        {
            this.CreateMonsters(context, gameConfiguration);
            var mapDefinition = context.CreateNew<GameMapDefinition>();
            mapDefinition.Number = this.MapNumber;
            mapDefinition.Name = this.MapName;
            mapDefinition.TerrainData = Terrains.ResourceManager.GetObject("Terrain" + (this.MapNumber + 1)) as byte[];
            mapDefinition.ExpMultiplier = 1;
            foreach (var spawn in this.CreateSpawns(context, mapDefinition, gameConfiguration))
            {
                mapDefinition.MonsterSpawns.Add(spawn);
            }

            return mapDefinition;
        }

        /// <summary>
        /// Creates all monster spawn areas.
        /// </summary>
        /// <param name="context">The persistence context.</param>
        /// <param name="mapDefinition">The game map definition.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        /// <returns>
        /// The spawn areas of the game map.
        /// </returns>
        /// <remarks>
        /// Can be extracted from MonsterSetBase.txt by Regex:
        /// Search (single): (?m)^(\d+)[ \t]+(\d+)[ \t]+(\d+)[ \t]+(\d+)[ \t]+(\d+)[ \t]+((-|)\d+).*?$
        /// Replace by (single): <![CDATA[yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[$1], 1, $6, SpawnTrigger.Automatic, $4, $4, $5, $5);]]>
        /// Search (multiple): (?m)^(\d+)\t*?(\d+)\t+?(\d+)\t+?(\d+)\t+?(\d+)\t+?(\d+)\t+?(\d+)\t+?(-*\d+)\t+?(\d+).*?$
        /// Replace by (multiple): <![CDATA[yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[$1], $9, 0, SpawnTrigger.Automatic, $4, $6, $5, $7);]]>
        /// </remarks>
        protected abstract IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration);

        /// <summary>
        /// Creates all map specific <see cref="MonsterDefinition"/>s and adds them to the gameConfiguration.
        /// </summary>
        /// <param name="context">The persistence context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        /// <remarks>
        /// Can be extracted from Monsters.txt by Regex: (?m)^(\d+)\t1\t"(.*?)"\t*?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+).*?$
        /// <![CDATA[Replace by:            {\r\n                var monster = context.CreateNew<MonsterDefinition>();\r\n                gameConfiguration.Monsters.Add(monster);\r\n                monster.Number = $1;\r\n                monster.Designation = "$2";\r\n                monster.MoveRange = $12;\r\n                monster.AttackRange = $14;\r\n                monster.ViewRange = $15;\r\n                monster.MoveDelay = new TimeSpan\($16 * TimeSpan.TicksPerMillisecond\);\r\n                monster.AttackDelay = new TimeSpan\($17 * TimeSpan.TicksPerMillisecond\);\r\n                monster.RespawnDelay = new TimeSpan\($18 * TimeSpan.TicksPerSecond\);\r\n                monster.Attribute = $19;\r\n                monster.NumberOfMaximumItemDrops = 1;\r\n                new Dictionary<AttributeDefinition, float>\r\n                {\r\n                    { Stats.Level, $3 },\r\n                    { Stats.MaximumHealth, $4 },\r\n                    { Stats.MinimumPhysBaseDmg, $6 },\r\n                    { Stats.MaximumPhysBaseDmg, $7 },\r\n                    { Stats.DefenseBase, $8 },\r\n                    { Stats.AttackRatePvm, $10 },\r\n                    { Stats.DefenseRatePvm, $11 },\r\n                    { Stats.WindResistance, $23 },\r\n                    { Stats.PoisonResistance, $24 },\r\n                    { Stats.IceResistance, $25 },\r\n                    { Stats.WaterResistance, $26 },\r\n                    { Stats.FireResistance, $27 },\r\n                }.Select(kvp =>\r\n                {\r\n                    var attribute = context.CreateNew<MonsterAttribute>();\r\n                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);\r\n                    attribute.Value = kvp.Value;\r\n                    return attribute;\r\n                }).ToList().ForEach(monster.Attributes.Add);\r\n            }\r\n]]>
        /// </remarks>
        protected abstract void CreateMonsters(IContext context, GameConfiguration gameConfiguration);

        /// <summary>
        /// Creates a new <see cref="MonsterSpawnArea"/> with the specified data.
        /// </summary>
        /// <param name="context">The persistence context.</param>
        /// <param name="map">The map.</param>
        /// <param name="monsterDefinition">The monster definition.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="spawnTrigger">The spawn trigger.</param>
        /// <param name="x1">The x1 coordinate.</param>
        /// <param name="x2">The x2 coordinate.</param>
        /// <param name="y1">The y1 coordinate.</param>
        /// <param name="y2">The y2 coordinate.</param>
        /// <returns>The created monster spawn area.</returns>
        protected MonsterSpawnArea CreateMonsterSpawn(IContext context, GameMapDefinition map, MonsterDefinition monsterDefinition, short quantity, byte direction, SpawnTrigger spawnTrigger, byte x1, byte x2, byte y1, byte y2)
        {
            var area = context.CreateNew<MonsterSpawnArea>();
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
    }
}
