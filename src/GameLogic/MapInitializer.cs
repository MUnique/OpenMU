// <copyright file="MapInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;
    using System.Linq;
    using log4net;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.NPC;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A basic map initializer.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.GameLogic.IMapInitializer" />
    public class MapInitializer : IMapInitializer
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MapInitializer));
        private readonly IDropGenerator defaultDropGenerator;
        private readonly GameConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapInitializer"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public MapInitializer(GameConfiguration configuration)
        {
            this.defaultDropGenerator = new DefaultDropGenerator(configuration, Rand.GetRandomizer());
            this.configuration = configuration;
            this.ItemDropDuration = 60;
            this.ChunkSize = 8;
        }

        /// <summary>
        /// Gets or sets the plug in manager.
        /// </summary>
        public PlugInManager PlugInManager { get; set; }

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
        public GameMap CreateGameMap(ushort mapNumber)
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
            Logger.Debug($"Start creating monster instances for map {createdMap}");
            foreach (var spawn in createdMap.Definition.MonsterSpawns.Where(s => s.SpawnTrigger == SpawnTrigger.Automatic))
            {
                for (int i = 0; i < spawn.Quantity; i++)
                {
                    var monsterDef = spawn.MonsterDefinition;
                    NonPlayerCharacter npc;

                    // TODO: Check if the condition is correct... NPCs are not attackable, but some might need to be (castle gates etc.). Also some NPCs are attacking, but should not be attackable (Traps).
                    if (monsterDef.AttackDelay > TimeSpan.Zero)
                    {
                        Logger.Debug($"Creating monster {spawn}");
                        npc = new Monster(spawn, monsterDef, createdMap, this.defaultDropGenerator, new BasicMonsterIntelligence(createdMap), this.PlugInManager);
                    }
                    else
                    {
                        Logger.Debug($"Creating npc {spawn}");
                        npc = new NonPlayerCharacter(spawn, monsterDef, createdMap);
                    }

                    npc.Initialize();
                    createdMap.Add(npc);
                }
            }

            Logger.Debug($"Finished creating monster instances for map {createdMap}");
        }

        /// <summary>
        /// Gets the map definition by searching for it at the <see cref="GameConfiguration"/>.
        /// </summary>
        /// <param name="mapNumber">The map number.</param>
        /// <returns>The game map definition.</returns>
        protected virtual GameMapDefinition GetMapDefinition(ushort mapNumber)
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
            return new GameMap(definition, this.ItemDropDuration, this.ChunkSize);
        }
    }
}