// <copyright file="BaseMapInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Maps
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Configuration.Items;

    /// <summary>
    /// Base class for a map initializer which provides some common basic functionality.
    /// </summary>
    internal abstract class BaseMapInitializer : IMapInitializer
    {
        private GameMapDefinition mapDefinition;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMapInitializer"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        protected BaseMapInitializer(IContext context, GameConfiguration gameConfiguration)
        {
            this.Context = context;
            this.GameConfiguration = gameConfiguration;
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected IContext Context { get; }

        /// <summary>
        /// Gets the game configuration.
        /// </summary>
        /// <value>
        /// The game configuration.
        /// </value>
        protected GameConfiguration GameConfiguration { get; }

        /// <summary>
        /// Gets the map number which will be set as <see cref="GameMapDefinition.Number"/>.
        /// </summary>
        protected abstract byte MapNumber { get; }

        /// <summary>
        /// Gets the name of the map which will be set as <see cref="GameMapDefinition.Name"/>.
        /// </summary>
        protected abstract string MapName { get; }

        /// <inheritdoc />
        public void Initialize()
        {
            this.CreateMonsters();
            this.mapDefinition = this.Context.CreateNew<GameMapDefinition>();
            this.mapDefinition.Number = this.MapNumber;
            this.mapDefinition.Name = this.MapName;
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetName().Name + ".Resources.Terrain" + (this.MapNumber + 1) + ".att";
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        this.mapDefinition.TerrainData = reader.ReadBytes((int)stream.Length);
                    }
                }
            }

            this.mapDefinition.ExpMultiplier = 1;
            foreach (var spawn in this.CreateSpawns())
            {
                this.mapDefinition.MonsterSpawns.Add(spawn);
            }

            this.CreateMapAttributeRequirements();

            this.InitializeDropItemGroups();
            this.GameConfiguration.Maps.Add(this.mapDefinition);
        }

        /// <inheritdoc/>
        public virtual void SetSafezoneMap()
        {
            if (this.mapDefinition.ExitGates.Any(g => g.IsSpawnGate))
            {
                this.mapDefinition.SafezoneMap = this.mapDefinition;
            }
            else
            {
                var lorencia = this.GameConfiguration.Maps.FirstOrDefault(map => map.Number == Lorencia.Number);
                this.mapDefinition.SafezoneMap = lorencia;
            }
        }

        /// <summary>
        /// Initializes the drop item groups for this map.
        /// By default, we add money and random items. On event or special maps, this can be overwritten.
        /// </summary>
        protected virtual void InitializeDropItemGroups()
        {
            this.mapDefinition.DropItemGroups.Add(this.GameConfiguration.DropItemGroups.FirstOrDefault(g => g.ItemType == SpecialItemType.Money));
            this.mapDefinition.DropItemGroups.Add(this.GameConfiguration.DropItemGroups.FirstOrDefault(g => g.ItemType == SpecialItemType.RandomItem));
            this.mapDefinition.DropItemGroups.Add(this.GameConfiguration.DropItemGroups.FirstOrDefault(g => g.ItemType == SpecialItemType.Excellent));
        }

        /// <summary>
        /// Creates all monster spawn areas.
        /// </summary>
        /// <returns>
        /// The spawn areas of the game map.
        /// </returns>
        /// <remarks>
        /// Can be extracted from MonsterSetBase.txt by Regex:
        /// Search (single): (?m)^(\d+)[ \t]+(\d+)[ \t]+(\d+)[ \t]+(\d+)[ \t]+(\d+)[ \t]+((-|)\d+).*?$
        /// Replace by (single): <![CDATA[yield return this.CreateMonsterSpawn(npcDictionary[$1], $4, $5, (Direction)$6, SpawnTrigger.Automatic);]]>
        /// Search (multiple): (?m)^(\d+)\t*?(\d+)\t+?(\d+)\t+?(\d+)\t+?(\d+)\t+?(\d+)\t+?(\d+)\t+?(-*\d+)\t+?(\d+).*?$
        /// Replace by (multiple): <![CDATA[yield return this.CreateMonsterSpawn(npcDictionary[$1], $4, $6, $5, $7, $9, Direction.Undefined, SpawnTrigger.Automatic);]]>
        /// </remarks>
        protected abstract IEnumerable<MonsterSpawnArea> CreateSpawns();

        /// <summary>
        /// Creates all map specific <see cref="MonsterDefinition"/>s and adds them to the gameConfiguration.
        /// </summary>
        /// <remarks>
        /// Can be extracted from Monsters.txt by Regex: (?m)^(\d+)\t1\t"(.*?)"\t*?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+).*?$
        /// <![CDATA[Replace by:            {\r\n                var monster = this.Context.CreateNew<MonsterDefinition>();\r\n                this.GameConfiguration.Monsters.Add(monster);\r\n                monster.Number = $1;\r\n                monster.Designation = "$2";\r\n                monster.MoveRange = $12;\r\n                monster.AttackRange = $14;\r\n                monster.ViewRange = $15;\r\n                monster.MoveDelay = new TimeSpan\($16 * TimeSpan.TicksPerMillisecond\);\r\n                monster.AttackDelay = new TimeSpan\($17 * TimeSpan.TicksPerMillisecond\);\r\n                monster.RespawnDelay = new TimeSpan\($18 * TimeSpan.TicksPerSecond\);\r\n                monster.Attribute = $19;\r\n                monster.NumberOfMaximumItemDrops = 1;\r\n                var attributes = new Dictionary<AttributeDefinition, float>\r\n                {\r\n                    { Stats.Level, $3 },\r\n                    { Stats.MaximumHealth, $4 },\r\n                    { Stats.MinimumPhysBaseDmg, $6 },\r\n                    { Stats.MaximumPhysBaseDmg, $7 },\r\n                    { Stats.DefenseBase, $8 },\r\n                    { Stats.AttackRatePvm, $10 },\r\n                    { Stats.DefenseRatePvm, $11 },\r\n                    { Stats.WindResistance, $23 },\r\n                    { Stats.PoisonResistance, $24 },\r\n                    { Stats.IceResistance, $25 },\r\n                    { Stats.WaterResistance, $26 },\r\n                    { Stats.FireResistance, $27 },\r\n                };\r\n                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);\r\n            }\r\n]]>
        /// </remarks>
        protected abstract void CreateMonsters();

        /// <summary>
        /// Creates a new <see cref="MonsterSpawnArea"/> with the specified data.
        /// </summary>
        /// <param name="monsterDefinition">The monster definition.</param>
        /// <param name="x1">The x1 coordinate.</param>
        /// <param name="x2">The x2 coordinate.</param>
        /// <param name="y1">The y1 coordinate.</param>
        /// <param name="y2">The y2 coordinate.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="spawnTrigger">The spawn trigger.</param>
        /// <returns>The created monster spawn area.</returns>
        protected MonsterSpawnArea CreateMonsterSpawn(MonsterDefinition monsterDefinition, byte x1, byte x2, byte y1, byte y2, short quantity = 1, Direction direction = Direction.Undefined, SpawnTrigger spawnTrigger = SpawnTrigger.Automatic)
        {
            var area = this.Context.CreateNew<MonsterSpawnArea>();
            area.GameMap = this.mapDefinition;
            area.MonsterDefinition = monsterDefinition;
            area.Quantity = quantity;
            area.Direction = direction;
            area.SpawnTrigger = spawnTrigger;
            area.X1 = x1;
            area.X2 = x2;
            area.Y1 = y1;
            area.Y2 = y2;
            return area;
        }

        /// <summary>
        /// Creates a new <see cref="MonsterSpawnArea"/> with the specified data.
        /// </summary>
        /// <param name="monsterDefinition">The monster definition.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="spawnTrigger">The spawn trigger.</param>
        /// <returns>The created monster spawn area.</returns>
        protected MonsterSpawnArea CreateMonsterSpawn(MonsterDefinition monsterDefinition, byte x, byte y, Direction direction = Direction.Undefined, SpawnTrigger spawnTrigger = SpawnTrigger.Automatic)
            => this.CreateMonsterSpawn(monsterDefinition, x, x, y, y, 1, direction, spawnTrigger);

        /// <summary>
        /// Can be used to add additional map requirements.
        /// </summary>
        protected virtual void CreateMapAttributeRequirements()
        {
            // needs to be overwritten if a requirement needs to be added.
        }

        /// <summary>
        /// Creates an attribute requirement with the specified minimum value.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="minimumValue">The minimum value.</param>
        protected void CreateRequirement(AttributeDefinition attribute, int minimumValue)
        {
            var requirement = this.Context.CreateNew<AttributeRequirement>();
            requirement.Attribute = attribute.GetPersistent(this.GameConfiguration);
            requirement.MinimumValue = minimumValue;
            this.mapDefinition.MapRequirements.Add(requirement);
        }
    }
}
