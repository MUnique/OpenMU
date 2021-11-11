﻿// <copyright file="MapInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A basic map initializer.
/// </summary>
/// <seealso cref="MUnique.OpenMU.GameLogic.IMapInitializer" />
public class MapInitializer : IMapInitializer
{
    private readonly IDropGenerator _dropGenerator;
    private readonly GameConfiguration _configuration;
    private readonly ILogger<MapInitializer> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapInitializer" /> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="dropGenerator">The drop generator.</param>
    public MapInitializer(GameConfiguration configuration, ILogger<MapInitializer> logger, IDropGenerator dropGenerator)
    {
        this._dropGenerator = dropGenerator;
        this._configuration = configuration;
        this._logger = logger;
        this.ItemDropDuration = 60;
        this.ChunkSize = 8;
    }

    /// <summary>
    /// Gets or sets the plug in manager.
    /// </summary>
    public PlugInManager? PlugInManager { get; set; }

    /// <summary>
    /// Gets or sets the duration of the item drop on created <see cref="GameMap"/>s.
    /// </summary>
    /// <value>
    /// The duration of the item drop on created <see cref="GameMap"/>s.
    /// </value>
    protected int ItemDropDuration { get; set; }

    /// <summary>
    /// Gets or sets the size of the chunk of created <see cref="GameMap"/>s.
    /// </summary>
    /// <value>
    /// The size of the chunk of created <see cref="GameMap"/>s.
    /// </value>
    protected byte ChunkSize { get; set; }

    /// <inheritdoc/>
    public GameMap? CreateGameMap(ushort mapNumber)
    {
        var definition = this.GetMapDefinition(mapNumber);
        if (definition != null)
        {
            return this.InternalCreateGameMap(definition);
        }

        return null;
    }

    /// <summary>
    /// Creates a new game map instance with the specified definition.
    /// </summary>
    /// <param name="mapDefinition">The map definition.</param>
    /// <returns>The new game map instance.</returns>
    public GameMap CreateGameMap(GameMapDefinition mapDefinition)
    {
        return this.InternalCreateGameMap(mapDefinition);
    }

    /// <inheritdoc />
    public void InitializeState(GameMap createdMap)
    {
        if (this.PlugInManager is null)
        {
            throw new InvalidOperationException("PlugInManager must be set first");
        }

        this._logger.LogDebug("Start creating monster instances for map {createdMap}", createdMap);
        var automaticSpawns = createdMap.Definition.MonsterSpawns
            .Where(m => m.MonsterDefinition is not null)
            .Where(m => m.SpawnTrigger is SpawnTrigger.Automatic);
        foreach (var spawnArea in automaticSpawns)
        {
            for (int i = 0; i < spawnArea.Quantity; i++)
            {
                this.InitializeNpc(createdMap, spawnArea);
            }
        }

        this._logger.LogDebug("Finished creating monster instances for map {createdMap}", createdMap);
    }

    /// <summary>
    /// Initializes the event NPCs of the previously created game map.
    /// </summary>
    /// <param name="createdMap">The created map.</param>
    /// <param name="eventStateProvider">The event state provider.</param>
    public void InitializeNpcsOnEventStart(GameMap createdMap, IEventStateProvider eventStateProvider)
    {
        if (this.PlugInManager is null)
        {
            throw new InvalidOperationException("PlugInManager must be set first");
        }

        this._logger.LogDebug("Start creating event monster instances for map {createdMap}", createdMap);
        var eventSpawns = createdMap.Definition.MonsterSpawns
            .Where(m => m.MonsterDefinition is not null)
            .Where(m => m.SpawnTrigger is SpawnTrigger.OnceAtEventStart or SpawnTrigger.AutomaticDuringEvent);

        foreach (var spawnArea in eventSpawns)
        {
            for (int i = 0; i < spawnArea.Quantity; i++)
            {
                this.InitializeNpc(createdMap, spawnArea, eventStateProvider);
            }
        }

        this._logger.LogDebug("Finished creating event monster instances for map {createdMap}", createdMap);
    }

    /// <inheritdoc />
    public void InitializeNpcsOnWaveStart(GameMap createdMap, IEventStateProvider eventStateProvider, byte waveNumber)
    {
        if (this.PlugInManager is null)
        {
            throw new InvalidOperationException("PlugInManager must be set first");
        }

        this._logger.LogDebug("Start creating event monster instances for map {createdMap}", createdMap);
        var waveSpawns = createdMap.Definition.MonsterSpawns
            .Where(m => m.MonsterDefinition is not null)
            .Where(m => m.SpawnTrigger is SpawnTrigger.AutomaticDuringWave or SpawnTrigger.OnceAtWaveStart)
            .Where(m => m.WaveNumber == waveNumber);

        foreach (var spawnArea in waveSpawns)
        {
            for (int i = 0; i < spawnArea.Quantity; i++)
            {
                this.InitializeNpc(createdMap, spawnArea, eventStateProvider);
            }
        }

        this._logger.LogDebug("Finished creating event monster instances for map {createdMap}", createdMap);
    }

    /// <summary>
    /// Gets the map definition by searching for it at the <see cref="GameConfiguration"/>.
    /// </summary>
    /// <param name="mapNumber">The map number.</param>
    /// <returns>The game map definition.</returns>
    protected virtual GameMapDefinition? GetMapDefinition(ushort mapNumber)
    {
        return this._configuration.Maps.FirstOrDefault(m => m.Number == mapNumber);
    }

    /// <summary>
    /// Creates the game map instance with the specified definition.
    /// </summary>
    /// <param name="definition">The definition.</param>
    /// <returns>
    /// The created game map instance.
    /// </returns>
    protected virtual GameMap InternalCreateGameMap(GameMapDefinition definition)
    {
        this._logger.LogDebug("Creating GameMap {0}", definition);
        return definition.BattleZone?.Type == BattleType.Soccer
            ? new SoccerGameMap(definition, this.ItemDropDuration, this.ChunkSize)
            : new GameMap(definition, this.ItemDropDuration, this.ChunkSize);
    }

    private void InitializeNpc(GameMap createdMap, MonsterSpawnArea spawnArea, IEventStateProvider? eventStateProvider = null)
    {
        var monsterDef = spawnArea.MonsterDefinition!;
        NonPlayerCharacter npc;

        var intelligence = this.TryCreateConfiguredNpcIntelligence(monsterDef, createdMap);

        if (monsterDef.ObjectKind == NpcObjectKind.Monster)
        {
            this._logger.LogDebug("Creating monster {spawn}", spawnArea);
            npc = new Monster(spawnArea, monsterDef, createdMap, this._dropGenerator, intelligence ?? new BasicMonsterIntelligence(), this.PlugInManager!, eventStateProvider);
        }
        else if (monsterDef.ObjectKind == NpcObjectKind.Trap)
        {
            this._logger.LogDebug("Creating trap {spawn}", spawnArea);
            npc = new Trap(spawnArea, monsterDef, createdMap, intelligence ?? new RandomAttackInRangeTrapIntelligence(createdMap));
        }
        else if (monsterDef.ObjectKind == NpcObjectKind.SoccerBall)
        {
            this._logger.LogDebug("Creating soccer ball {spawn}", spawnArea);
            npc = new SoccerBall(spawnArea, monsterDef, createdMap);
        }
        else
        {
            this._logger.LogDebug("Creating npc {spawn}", spawnArea);
            npc = new NonPlayerCharacter(spawnArea, monsterDef, createdMap);
        }

        try
        {
            npc.Initialize();
            createdMap.Add(npc);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, $"Object {spawnArea} couldn't be initialized.", spawnArea);
            npc.Dispose();
        }
    }

    private INpcIntelligence? TryCreateConfiguredNpcIntelligence(MonsterDefinition monsterDefinition, GameMap createdMap)
    {
        if (string.IsNullOrWhiteSpace(monsterDefinition.IntelligenceTypeName))
        {
            return null;
        }

        try
        {
            var type = Type.GetType(monsterDefinition.IntelligenceTypeName);
            if (type is null)
            {
                this._logger.LogError($"Could not find type {monsterDefinition.IntelligenceTypeName}");
                return null;
            }

            return Activator.CreateInstance(type, createdMap) as INpcIntelligence;
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, $"Could not create npc intelligence for monster {monsterDefinition.Designation}, type name {monsterDefinition.IntelligenceTypeName}");
        }

        return null;
    }
}