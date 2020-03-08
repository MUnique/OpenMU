// <copyright file="RepositoryManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System;
    using System.Linq;
    using log4net;
    using MUnique.OpenMU.Persistence.EntityFramework.Json;

    /// <summary>
    /// A repository manager which utilizes repositories which use the entity framework core to do data access.
    /// </summary>
    /// <remarks>
    /// We create the most repositories by the following convention:
    ///   - Configuration data repositories: We create repositories which retrieve data from the current GameConfiguration to save memory and less database queries.
    ///   - Entity data (Accounts, etc.): We create repositories which retrieves every object at every access from the database.
    /// </remarks>
    public class RepositoryManager : BaseRepositoryManager
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PersistenceContextProvider));

        /// <summary>
        /// Registers the repositories.
        /// </summary>
        /// <param name="contextProvider">The context provider which is required in most of the repositories.</param>
        public void RegisterRepositories(PersistenceContextProvider contextProvider)
        {
            this.RegisterRepository(new AccountRepository(contextProvider));
            this.RegisterRepository(new LetterBodyRepository(contextProvider));
            this.RegisterRepository(new CachedRepository<GameConfiguration>(new GameConfigurationRepository(contextProvider)));
            this.RegisterRepository(new GenericRepository<GameServerConfiguration>(contextProvider));
            this.RegisterRepository(new GenericRepository<GameClientDefinition>(contextProvider));
            this.RegisterRepository(new GenericRepository<ConnectServerDefinition>(contextProvider));
            this.RegisterRepository(new GenericRepository<ChatServerDefinition>(contextProvider));
            this.RegisterRepository(new GenericRepository<ChatServerEndpoint>(contextProvider));
            this.RegisterRepository(new GenericRepository<GameServerEndpoint>(contextProvider));
            this.RegisterRepository(new GameServerDefinitionRepository(contextProvider));

            this.RegisterRepository(new ConfigurationTypeRepository<ItemOptionDefinition>(contextProvider, config => config.RawItemOptions));
            this.RegisterRepository(new ConfigurationTypeRepository<IncreasableItemOption>(
                contextProvider,
                config => config.RawItemOptions.SelectMany(o => o.RawPossibleOptions).Concat(config.RawItemSetGroups.SelectMany(g => g.RawOptions)).Distinct().ToList()));
            this.RegisterRepository(new ConfigurationTypeRepository<AttributeDefinition>(contextProvider, config => config.RawAttributes));
            this.RegisterRepository(new ConfigurationTypeRepository<DropItemGroup>(contextProvider, config => config.RawDropItemGroups));
            this.RegisterRepository(new ConfigurationTypeRepository<CharacterClass>(contextProvider, config => config.RawCharacterClasses));
            this.RegisterRepository(new ConfigurationTypeRepository<ItemOptionType>(contextProvider, config => config.RawItemOptionTypes));
            this.RegisterRepository(new ConfigurationTypeRepository<ItemSetGroup>(contextProvider, config => config.RawItemSetGroups));
            this.RegisterRepository(new ConfigurationTypeRepository<ItemSlotType>(contextProvider, config => config.RawItemSlotTypes));
            this.RegisterRepository(new ConfigurationTypeRepository<ItemDefinition>(contextProvider, config => config.RawItems));
            this.RegisterRepository(new ConfigurationTypeRepository<JewelMix>(contextProvider, config => config.RawJewelMixes));
            this.RegisterRepository(new ConfigurationTypeRepository<MagicEffectDefinition>(contextProvider, config => config.RawMagicEffects));
            this.RegisterRepository(new ConfigurationTypeRepository<GameMapDefinition>(contextProvider, config => config.RawMaps));
            this.RegisterRepository(new ConfigurationTypeRepository<MasterSkillRoot>(contextProvider, config => config.RawMasterSkillRoots));
            this.RegisterRepository(new ConfigurationTypeRepository<MonsterDefinition>(contextProvider, config => config.RawMonsters));
            this.RegisterRepository(new ConfigurationTypeRepository<Skill>(contextProvider, config => config.RawSkills));
            this.RegisterRepository(new ConfigurationTypeRepository<PlugInConfiguration>(contextProvider, config => config.RawPlugInConfigurations));

            var registeredTypes = this.Repositories.Keys.ToList();
            using var entityContext = new EntityDataContext();
            var modelTypes = entityContext.Model.GetEntityTypes().Select(e => e.ClrType);
            var missingTypes = modelTypes.Where(t => !registeredTypes.Contains(t.BaseType));
            foreach (var type in missingTypes)
            {
                Log.Debug($"No repository registered for type {type}, creating a generic one.");
                var repositoryType = typeof(GenericRepository<>).MakeGenericType(type);
                var repository = Activator.CreateInstance(repositoryType, contextProvider) as IRepository;

                this.RegisterRepository(type, repository);
            }
        }

        /// <summary>
        /// Ensures the caches for current game configuration.
        /// It's meant to fill the caches also in <see cref="ConfigurationIdReferenceResolver"/>.
        /// </summary>
        public void EnsureCachesForCurrentGameConfiguration()
        {
            foreach (var repository in this.Repositories.Values.OfType<IConfigurationTypeRepository>())
            {
                repository.EnsureCacheForCurrentConfiguration();
            }
        }

        /// <summary>
        /// Registers the repository. Adapts the type, so that the base type gets registered.
        /// </summary>
        /// <param name="type">The generic type which the repository handles.</param>
        /// <param name="repository">The repository.</param>
        protected override void RegisterRepository(Type type, IRepository repository)
        {
            if (type.Namespace == this.GetType().Namespace && type.BaseType != typeof(object))
            {
                base.RegisterRepository(type.BaseType, repository);
            }
            else
            {
                base.RegisterRepository(type, repository);
            }
        }
    }
}