// <copyright file="BaseMapInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using MUnique.OpenMU.Network;
using MUnique.OpenMU.Persistence.Initialization.Updates;

namespace MUnique.OpenMU.Persistence.Initialization;

using System.Diagnostics;
using System.IO;
using System.Reflection;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic;

/// <summary>
/// Base class for a map initializer which provides some common basic functionality.
/// </summary>
internal abstract class BaseMapInitializer : IMapInitializer
{
    private static readonly IList<DropItemGroup> DefaultDropItemGroups = new List<DropItemGroup>();

    private GameMapDefinition? _mapDefinition;

    private Dictionary<short, MonsterDefinition>? _npcDictionary;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseMapInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    protected BaseMapInitializer(IContext context, GameConfiguration gameConfiguration)
    {
        this.Context = context;
        this.GameConfiguration = gameConfiguration;
        this._mapDefinition = this.GameConfiguration.Maps.FirstOrDefault(map => map.Number == this.MapNumber && map.Discriminator == this.Discriminator);
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
    /// Gets the map definition.
    /// </summary>
    protected GameMapDefinition? MapDefinition => this._mapDefinition;

    /// <summary>
    /// Gets the NPC dictionary.
    /// </summary>
    protected Dictionary<short, MonsterDefinition> NpcDictionary => this._npcDictionary ??= this.GameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

    /// <summary>
    /// Gets the map number which will be set as <see cref="GameMapDefinition.Number"/>.
    /// </summary>
    protected abstract byte MapNumber { get; }

    /// <summary>
    /// Gets the name of the map which will be set as <see cref="GameMapDefinition.Name"/>.
    /// </summary>
    protected abstract string MapName { get; }

    /// <summary>
    /// Gets the version prefix for Terrain ressources.
    /// </summary>
    protected virtual string TerrainVersionPrefix => string.Empty;

    /// <summary>
    /// Gets the discriminator of the map definition.
    /// </summary>
    protected virtual byte Discriminator { get; }

    /// <summary>
    /// Gets the map number of the safezone map where a player respawns after death.
    /// </summary>
    protected virtual byte SafezoneMapNumber => (byte)(this._mapDefinition?.ExitGates.Any(g => g.IsSpawnGate) ?? false ? this._mapDefinition.Number : Version075.Maps.Lorencia.Number);

    private short MapId
    {
        get
        {
            if (this.MapDefinition!.Discriminator is 0)
            {
                return this.MapDefinition.Number;
            }

            return NumberConversionExtensions.MakeWord(
                (byte)this._mapDefinition!.Number,
                (byte)this._mapDefinition.Discriminator).ToSigned();
        }
    }

    /// <inheritdoc />
    public void Initialize()
    {
        this.CreateMonsters();
        this._mapDefinition = this.Context.CreateNew<GameMapDefinition>();
        this._mapDefinition.SetGuid(this.MapNumber, this.Discriminator);
        this._mapDefinition.Number = this.MapNumber;
        this._mapDefinition.Name = this.MapName;
        this._mapDefinition.Discriminator = this.Discriminator;
        this._mapDefinition.UpdateTerrainFromResources(this.TerrainVersionPrefix);

        this._mapDefinition.ExpMultiplier = 1;
        foreach (var spawn in this.CreateNpcSpawns().Concat(this.CreateMonsterSpawns()))
        {
            this._mapDefinition.MonsterSpawns.Add(spawn);
        }

        this.CreateMapAttributeRequirements();

        this.InitializeDropItemGroups();
        this.AdditionalInitialization(this._mapDefinition);
        this.GameConfiguration.Maps.Add(this._mapDefinition);
    }

    /// <inheritdoc/>
    public void SetSafezoneMap()
    {
        if (this._mapDefinition is null)
        {
            throw new InvalidOperationException("Map is not initialized yet.");
        }

        var safezoneMap = this.GameConfiguration.Maps.FirstOrDefault(map => map.Number == this.SafezoneMapNumber);
        this._mapDefinition.SafezoneMap = safezoneMap;
    }

    /// <summary>
    /// Registers the default drop item group.
    /// </summary>
    /// <param name="dropItemGroup">The drop item group.</param>
    internal static void RegisterDefaultDropItemGroup(DropItemGroup dropItemGroup)
    {
        DefaultDropItemGroups.Add(dropItemGroup);
    }

    /// <summary>
    /// Clears the default drop item groups.
    /// </summary>
    internal static void ClearDefaultDropItemGroups() => DefaultDropItemGroups.Clear();

    /// <summary>
    /// Does additional initialization.
    /// </summary>
    /// <param name="mapDefinition">The map definition.</param>
    protected virtual void AdditionalInitialization(GameMapDefinition mapDefinition)
    {
        // can be overwritten
    }

    /// <summary>
    /// Initializes the drop item groups for this map.
    /// By default, we add money and random items. On event or special maps, this can be overwritten.
    /// </summary>
    protected virtual void InitializeDropItemGroups()
    {
        if (this._mapDefinition is null)
        {
            throw new InvalidOperationException("MapDefiniton not set yet.");
        }

        DefaultDropItemGroups.ForEach(this._mapDefinition.DropItemGroups.Add);
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
    /// Replace by (single): <![CDATA[yield return this.CreateMonsterSpawn(NpcDictionary[$1], $4, $5, (Direction)$6, SpawnTrigger.Automatic);]]>
    /// Search (multiple): (?m)^(\d+)\t*?(\d+)\t+?(\d+)\t+?(\d+)\t+?(\d+)\t+?(\d+)\t+?(\d+)\t+?(-*\d+)\t+?(\d+).*?$
    /// Replace by (multiple): <![CDATA[yield return this.CreateMonsterSpawn(NpcDictionary[$1], $4, $6, $5, $7, $9, Direction.Undefined, SpawnTrigger.Automatic);]]>.
    /// </remarks>
    protected virtual IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        // can be overwritten to add Monsters
        yield break;
    }

    /// <summary>
    /// Creates all NPC spawns.
    /// </summary>
    /// <returns>
    /// The spawn areas of the game map.
    /// </returns>
    protected virtual IEnumerable<MonsterSpawnArea> CreateNpcSpawns()
    {
        // can be overwritten to add NPCs
        yield break;
    }

    /// <summary>
    /// Creates all map specific <see cref="MonsterDefinition"/>s and adds them to the gameConfiguration.
    /// </summary>
    /// <remarks>
    /// Can be extracted from Monsters.txt by Regex: (?m)^(\d+)\t1\t"(.*?)"\t*?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+)\t?(\d+).*?$.
    /// <![CDATA[Replace by:            {\r\n                var monster = this.Context.CreateNew<MonsterDefinition>();\r\n                this.GameConfiguration.Monsters.Add(monster);\r\n                monster.Number = $1;\r\n                monster.Designation = "$2";\r\n                monster.MoveRange = $12;\r\n                monster.AttackRange = $14;\r\n                monster.ViewRange = $15;\r\n                monster.MoveDelay = new TimeSpan\($16 * TimeSpan.TicksPerMillisecond\);\r\n                monster.AttackDelay = new TimeSpan\($17 * TimeSpan.TicksPerMillisecond\);\r\n                monster.RespawnDelay = new TimeSpan\($18 * TimeSpan.TicksPerSecond\);\r\n                monster.Attribute = $19;\r\n                monster.NumberOfMaximumItemDrops = 1;\r\n                var attributes = new Dictionary<AttributeDefinition, float>\r\n                {\r\n                    { Stats.Level, $3 },\r\n                    { Stats.MaximumHealth, $4 },\r\n                    { Stats.MinimumPhysBaseDmg, $6 },\r\n                    { Stats.MaximumPhysBaseDmg, $7 },\r\n                    { Stats.DefenseBase, $8 },\r\n                    { Stats.AttackRatePvm, $10 },\r\n                    { Stats.DefenseRatePvm, $11 },\r\n                    { Stats.WindResistance, $23f / 255 },\r\n                    { Stats.PoisonResistance, $24f / 255 },\r\n                    { Stats.IceResistance, $25f / 255 },\r\n                    { Stats.WaterResistance, $26f / 255 },\r\n                    { Stats.FireResistance, $27f / 255 },\r\n                };\r\n                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);\r\n            }\r\n]]>
    /// </remarks>
    protected virtual void CreateMonsters()
    {
        // can be overwritten to create new monster definitions
    }

    /// <summary>
    /// Creates a new <see cref="MonsterSpawnArea" /> with the specified data.
    /// </summary>
    /// <param name="number">The number of the spawn for later reference.</param>
    /// <param name="monsterDefinition">The monster definition.</param>
    /// <param name="x1">The x1 coordinate.</param>
    /// <param name="x2">The x2 coordinate.</param>
    /// <param name="y1">The y1 coordinate.</param>
    /// <param name="y2">The y2 coordinate.</param>
    /// <param name="quantity">The quantity.</param>
    /// <param name="direction">The direction.</param>
    /// <param name="spawnTrigger">The spawn trigger.</param>
    /// <param name="waveNumber">The wave number.</param>
    /// <returns>
    /// The created monster spawn area.
    /// </returns>
    protected MonsterSpawnArea CreateMonsterSpawn(short number, MonsterDefinition monsterDefinition, byte x1, byte x2, byte y1, byte y2, short quantity = 1, Direction direction = Direction.Undefined, SpawnTrigger spawnTrigger = SpawnTrigger.Automatic, byte waveNumber = 0)
    {
        var area = this.Context.CreateNew<MonsterSpawnArea>();
        area.SetGuid(this.MapId, number);
        area.GameMap = this._mapDefinition;
        area.MonsterDefinition = monsterDefinition;
        area.Quantity = quantity;
        area.Direction = direction;
        area.SpawnTrigger = spawnTrigger;
        area.X1 = x1;
        area.X2 = x2;
        area.Y1 = y1;
        area.Y2 = y2;
        area.WaveNumber = waveNumber;
        return area;
    }

    /// <summary>
    /// Creates a new <see cref="MonsterSpawnArea" /> with the specified data.
    /// </summary>
    /// <param name="number">The number.</param>
    /// <param name="monsterDefinition">The monster definition.</param>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="direction">The direction.</param>
    /// <param name="spawnTrigger">The spawn trigger.</param>
    /// <returns>
    /// The created monster spawn area.
    /// </returns>
    protected MonsterSpawnArea CreateMonsterSpawn(short number, MonsterDefinition monsterDefinition, byte x, byte y, Direction direction = Direction.Undefined, SpawnTrigger spawnTrigger = SpawnTrigger.Automatic)
        => this.CreateMonsterSpawn(number, monsterDefinition, x, x, y, y, 1, direction, spawnTrigger);

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
        if (this._mapDefinition is null)
        {
            throw new InvalidOperationException("MapDefinition not set yet.");
        }

        var requirement = this.Context.CreateNew<AttributeRequirement>();
        requirement.SetGuid(this.MapId, attribute.Id.ExtractFirstTwoBytes());
        requirement.Attribute = attribute.GetPersistent(this.GameConfiguration);
        requirement.MinimumValue = minimumValue;
        this._mapDefinition.MapRequirements.Add(requirement);
    }

    private string? GetTerrainFileName()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceNames = assembly.GetManifestResourceNames();
        for (var mapNumber = this.MapNumber + 1; mapNumber > 0 && mapNumber > this.MapNumber - 10; mapNumber--)
        {
            var candidate = $"{assembly.GetName().Name}.Resources.{this.TerrainVersionPrefix}Terrain{mapNumber}{(this.Discriminator > 0 ? ("_" + this.Discriminator) : string.Empty)}.att";
            if (resourceNames.Contains(candidate))
            {
                return candidate;
            }

            if (this.Discriminator > 0)
            {
                var candidate2 = $"{assembly.GetName().Name}.Resources.{this.TerrainVersionPrefix}Terrain{mapNumber}.att";
                if (resourceNames.Contains(candidate2))
                {
                    return candidate2;
                }
            }

            if (!char.IsDigit(this.MapName[^1]))
            {
                break;
            }
        }

        return null;
    }
}