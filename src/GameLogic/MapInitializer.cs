// <copyright file="MapInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;
    using System.Linq;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.NPC;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A basic map initializer.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.GameLogic.IMapInitializer" />
    public class MapInitializer : IMapInitializer
    {
        private readonly IDropGenerator defaultDropGenerator;
        private readonly GameConfiguration configuration;
        private readonly ILogger<MapInitializer> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapInitializer" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="logger">The logger.</param>
        public MapInitializer(GameConfiguration configuration, ILogger<MapInitializer> logger)
        {
            this.defaultDropGenerator = new DefaultDropGenerator(configuration, Rand.GetRandomizer());
            this.configuration = configuration;
            this.logger = logger;
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

        /// <inheritdoc />
        public void InitializeState(GameMap createdMap)
        {
            if (this.PlugInManager is null)
            {
                throw new InvalidOperationException("PlugInManager must be set first");
            }

            this.logger.LogDebug("Start creating monster instances for map {createdMap}", createdMap);
            foreach (var spawn in createdMap.Definition.MonsterSpawns.Where(s => s.SpawnTrigger == SpawnTrigger.Automatic && s.MonsterDefinition is not null))
            {
                for (int i = 0; i < spawn.Quantity; i++)
                {
                    var monsterDef = spawn.MonsterDefinition!;
                    NonPlayerCharacter npc;

                    var intelligence = this.TryCreateConfiguredNpcIntelligence(monsterDef, createdMap);

                    if (monsterDef.ObjectKind == NpcObjectKind.Monster)
                    {
                        this.logger.LogDebug("Creating monster {spawn}", spawn);
                        npc = new Monster(spawn, monsterDef, createdMap, this.defaultDropGenerator, intelligence ?? new BasicMonsterIntelligence(), this.PlugInManager);
                    }
                    else if (monsterDef.ObjectKind == NpcObjectKind.Trap)
                    {
                        this.logger.LogDebug("Creating trap {spawn}", spawn);
                        npc = new Trap(spawn, monsterDef, createdMap, intelligence ?? new RandomAttackInRangeTrapIntelligence(createdMap));
                    }
                    else if (monsterDef.ObjectKind == NpcObjectKind.SoccerBall)
                    {
                        this.logger.LogDebug("Creating soccer ball {spawn}", spawn);
                        npc = new SoccerBall(spawn, monsterDef, createdMap);
                    }
                    else
                    {
                        this.logger.LogDebug("Creating npc {spawn}", spawn);
                        npc = new NonPlayerCharacter(spawn, monsterDef, createdMap);
                    }

                    try
                    {
                        npc.Initialize();
                        createdMap.Add(npc);
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogError(ex, $"Object {spawn} couldn't be initialized.", spawn);
                        npc.Dispose();
                    }
                }
            }

            this.logger.LogDebug("Finished creating monster instances for map {createdMap}", createdMap);
        }

        /// <summary>
        /// Gets the map definition by searching for it at the <see cref="GameConfiguration"/>.
        /// </summary>
        /// <param name="mapNumber">The map number.</param>
        /// <returns>The game map definition.</returns>
        protected virtual GameMapDefinition? GetMapDefinition(ushort mapNumber)
        {
            return this.configuration.Maps.FirstOrDefault(m => m.Number == mapNumber);
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
            this.logger.LogDebug("Creating GameMap {0}", definition);
            return definition.BattleZone?.Type == BattleType.Soccer
                ? new SoccerGameMap(definition, this.ItemDropDuration, this.ChunkSize)
                : new GameMap(definition, this.ItemDropDuration, this.ChunkSize);
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
                    this.logger.LogError($"Could not find type {monsterDefinition.IntelligenceTypeName}");
                    return null;
                }

                return Activator.CreateInstance(type, createdMap) as INpcIntelligence;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Could not create npc intelligence for monster {monsterDefinition.Designation}, type name {monsterDefinition.IntelligenceTypeName}");
            }

            return null;
        }
    }
}