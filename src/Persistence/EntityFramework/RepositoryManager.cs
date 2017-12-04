// <copyright file="RepositoryManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using log4net;
    using Microsoft.EntityFrameworkCore;
    using Npgsql.Logging;

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
        private static readonly ILog Log = LogManager.GetLogger(nameof(RepositoryManager));

        /// <summary>
        /// A cache which holds extended types (Value) for their corresponding base type (Key).
        /// </summary>
        private readonly IDictionary<Type, Type> efCoreTypes = new Dictionary<Type, Type>();

        /// <summary>
        /// Registers the repositories.
        /// </summary>
        public void RegisterRepositories()
        {
            this.RegisterRepository(new AccountRepository(this));
            this.RegisterRepository(new GuildRepository(this));
            this.RegisterRepository(new FriendViewItemRepository());
            this.RegisterRepository(new CachedRepository<GameConfiguration>(new GameConfigurationRepository(this)));
            this.RegisterRepository(new GenericRepository<GameServerConfiguration>(this));
            this.RegisterRepository(new GameServerDefinitionRepository(this));
            this.RegisterRepository(new GenericRepository<MainPacketHandlerConfiguration>(this));
            this.RegisterRepository(new GenericRepository<PacketHandlerConfiguration>(this));

            this.RegisterRepository(new ConfigurationTypeRepository<ItemOptionDefinition>(this, config => config.RawItemOptions));
            this.RegisterRepository(new ConfigurationTypeRepository<AttributeDefinition>(this, config => config.RawAttributes));
            this.RegisterRepository(new ConfigurationTypeRepository<DropItemGroup>(this, config => config.RawBaseDropItemGroups));
            this.RegisterRepository(new ConfigurationTypeRepository<CharacterClass>(this, config => config.RawCharacterClasses));
            this.RegisterRepository(new ConfigurationTypeRepository<ItemOptionType>(this, config => config.RawItemOptionTypes));
            this.RegisterRepository(new ConfigurationTypeRepository<ItemSetGroup>(this, config => config.RawItemSetGroups));
            this.RegisterRepository(new ConfigurationTypeRepository<ItemSlotType>(this, config => config.RawItemSlotTypes));
            this.RegisterRepository(new ConfigurationTypeRepository<ItemDefinition>(this, config => config.RawItems));
            this.RegisterRepository(new ConfigurationTypeRepository<JewelMix>(this, config => config.RawJewelMixes));
            this.RegisterRepository(new ConfigurationTypeRepository<MagicEffectDefinition>(this, config => config.RawMagicEffects));
            this.RegisterRepository(new GameMapDefinitionRepository(this, config => config.RawMaps));
            this.RegisterRepository(new ConfigurationTypeRepository<MasterSkillRoot>(this, config => config.RawMasterSkillRoots));
            this.RegisterRepository(new ConfigurationTypeRepository<MonsterDefinition>(this, config => config.RawMonsters));
            this.RegisterRepository(new ConfigurationTypeRepository<Skill>(this, config => config.RawSkills));

            var registeredTypes = this.Repositories.Keys.ToList();
            using (var entityContext = new EntityDataContext())
            {
                var modelTypes = entityContext.Model.GetEntityTypes().Select(e => e.ClrType);
                var missingTypes = modelTypes.Where(t => !registeredTypes.Contains(t.BaseType) && !(t.BaseType?.Namespace?.Contains("Configuration") ?? false)).ToList();
                foreach (var type in missingTypes)
                {
                    Log.Debug($"No repository registered for type {type}, creating a generic one.");
                    var repositoryType = typeof(GenericRepository<>).MakeGenericType(type);
                    var repository = Activator.CreateInstance(repositoryType, this as IRepositoryManager) as IRepository;

                    this.RegisterRepository(type, repository);
                }
            }
        }

        /// <summary>
        /// Determines whether the database schema is up to date.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is database up to date]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsDatabaseUpToDate()
        {
            using (var installationContext = new EntityDataContext())
            {
                return !installationContext.Database.GetPendingMigrations().Any();
            }
        }

        /// <summary>
        /// Applies all pending updates to the database schema.
        /// </summary>
        public void ApplyAllPendingUpdates()
        {
            using (var installationContext = new EntityDataContext())
            {
                installationContext.Database.Migrate();
            }
        }

        /// <summary>
        /// Reinitializes the database by deleting it and running the initialization process again.
        /// </summary>
        public void ReInitializeDatabase()
        {
            using (var installationContext = new EntityDataContext())
            {
                installationContext.Database.EnsureDeleted();
                installationContext.Database.Migrate();
            }
        }

        /// <summary>
        /// Initializes the logging of sql statements.
        /// </summary>
        public void InitializeSqlLogging()
        {
            NpgsqlLogManager.Provider = new NpgsqlLog4NetLoggingProvider();
        }

        /// <inheritdoc />
        public override IContext CreateNewContext()
        {
            return new EntityFrameworkContext(new EntityDataContext());
        }

        /// <inheritdoc />
        public override IContext CreateNewContext(MUnique.OpenMU.DataModel.Configuration.GameConfiguration gameConfiguration)
        {
            return new EntityFrameworkContext(new EntityDataContext { CurrentGameConfiguration = gameConfiguration as GameConfiguration });
        }

        /// <inheritdoc />
        public override IContext CreateNewAccountContext(MUnique.OpenMU.DataModel.Configuration.GameConfiguration gameConfiguration)
        {
            return new EntityFrameworkContext(new AccountContext { CurrentGameConfiguration = gameConfiguration as GameConfiguration });
        }

        /// <inheritdoc />
        public override IContext CreateNewConfigurationContext()
        {
            return new EntityFrameworkContext(new ConfigurationContext());
        }

        /// <summary>
        /// Creates a new instance of <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type which should get created.</typeparam>
        /// <param name="args">The arguments which are handed 1-to-1 to the constructor. If no arguments are given, the default constructor will be called.</param>
        /// <returns>
        /// A new instance of <typeparamref name="T" />.
        /// </returns>
        public override T CreateNew<T>(params object[] args)
        {
            var instance = this.CreateNewInternal<T>(args);
            if (instance != null)
            {
                if (this.GetCurrentContext() is EntityFrameworkContext context)
                {
                    context.Context.Add(instance);
                    var repository = this.InternalGetRepository(typeof(T)) as INotifyAddedObject;
                    repository?.ObjectAdded(instance);
                }
                else
                {
                    throw new InvalidOperationException("CreateNew was called outside of a context-usage");
                }
            }

            return instance;
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
                type = type.BaseType;
            }

            base.RegisterRepository(type, repository);
        }

        private T CreateNewInternal<T>(params object[] args)
            where T : class
        {
            var efType = this.GetEfCoreTypeOf<T>();
            if (args.Length == 0)
            {
                return Activator.CreateInstance(efType) as T;
            }

            return this.CreateNew(efType, args) as T;
        }

        private Type GetEfCoreTypeOf<T>()
        {
            if (!this.efCoreTypes.TryGetValue(typeof(T), out Type efCoreType))
            {
                efCoreType = this.GetType().Assembly.GetTypes().First(t => typeof(T).IsAssignableFrom(t));
                this.efCoreTypes.Add(typeof(T), efCoreType);
            }

            return efCoreType;
        }
    }
}