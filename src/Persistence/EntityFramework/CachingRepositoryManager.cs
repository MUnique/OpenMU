// <copyright file="CachingRepositoryManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Persistence.EntityFramework.Json;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// A repository manager which utilizes repositories which use the entity framework core to do data access.
/// </summary>
/// <remarks>
/// We create the most repositories by the following convention:
///   - Configuration data repositories: We create repositories which retrieve data from the current GameConfiguration to save memory and less database queries.
///   - Entity data (Accounts, etc.): We create repositories which retrieves every object at every access from the database.
/// </remarks>
public class CachingRepositoryManager : RepositoryManager
{
    private readonly ILoggerFactory _loggerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="CachingRepositoryManager"/> class.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    public CachingRepositoryManager(ILoggerFactory loggerFactory)
        : base(loggerFactory, null)
    {
        this._loggerFactory = loggerFactory;
    }

    /// <summary>
    /// Registers the repositories.
    /// </summary>
    public override void RegisterRepositories()
    {
        this.RegisterRepository(new AccountRepository(this, this._loggerFactory.CreateLogger<AccountRepository>()));
        this.RegisterRepository(new LetterBodyRepository(this, this._loggerFactory.CreateLogger<LetterBodyRepository>()));
        this.RegisterRepository(new CachedRepository<GameConfiguration>(new CachingGameConfigurationRepository(this, this._loggerFactory.CreateLogger<CachingGameConfigurationRepository>())));
        this.RegisterRepository(new CachingGenericRepository<GameServerConfiguration>(this, this._loggerFactory.CreateLogger<CachingGenericRepository<GameServerConfiguration>>()));
        this.RegisterRepository(new CachingGenericRepository<GameClientDefinition>(this, this._loggerFactory.CreateLogger<CachingGenericRepository<GameClientDefinition>>()));
        this.RegisterRepository(new CachingGenericRepository<ConnectServerDefinition>(this, this._loggerFactory.CreateLogger<CachingGenericRepository<ConnectServerDefinition>>()));
        this.RegisterRepository(new CachingGenericRepository<ChatServerDefinition>(this, this._loggerFactory.CreateLogger<CachingGenericRepository<ChatServerDefinition>>()));
        this.RegisterRepository(new CachingGenericRepository<ChatServerEndpoint>(this, this._loggerFactory.CreateLogger<CachingGenericRepository<ChatServerEndpoint>>()));
        this.RegisterRepository(new CachingGenericRepository<GameServerEndpoint>(this, this._loggerFactory.CreateLogger<CachingGenericRepository<GameServerEndpoint>>()));
        this.RegisterRepository(new GameServerDefinitionRepository(this, this._loggerFactory.CreateLogger<GameServerDefinitionRepository>()));

        this.RegisterRepository(new ConfigurationTypeRepository<ItemOptionDefinition>(this, config => config.RawItemOptions));
        this.RegisterRepository(new ConfigurationTypeRepository<IncreasableItemOption>(
            this,
            config => config.RawItemOptions.SelectMany(o => o.RawPossibleOptions).Concat(config.RawItemSetGroups.SelectMany(g => g.RawOptions)).Distinct().ToList()));
        this.RegisterRepository(new ConfigurationTypeRepository<AttributeDefinition>(this, config => config.RawAttributes));
        this.RegisterRepository(new ConfigurationTypeRepository<DropItemGroup>(this, config => config.RawDropItemGroups));
        this.RegisterRepository(new ConfigurationTypeRepository<CharacterClass>(this, config => config.RawCharacterClasses));
        this.RegisterRepository(new ConfigurationTypeRepository<ItemOptionType>(this, config => config.RawItemOptionTypes));
        this.RegisterRepository(new ConfigurationTypeRepository<ItemSetGroup>(this, config => config.RawItemSetGroups));
        this.RegisterRepository(new ConfigurationTypeRepository<ItemSlotType>(this, config => config.RawItemSlotTypes));
        this.RegisterRepository(new ConfigurationTypeRepository<ItemDefinition>(this, config => config.RawItems));
        this.RegisterRepository(new ConfigurationTypeRepository<JewelMix>(this, config => config.RawJewelMixes));
        this.RegisterRepository(new ConfigurationTypeRepository<MagicEffectDefinition>(this, config => config.RawMagicEffects));
        this.RegisterRepository(new ConfigurationTypeRepository<GameMapDefinition>(this, config => config.RawMaps));
        this.RegisterRepository(new ConfigurationTypeRepository<MasterSkillRoot>(this, config => config.RawMasterSkillRoots));
        this.RegisterRepository(new ConfigurationTypeRepository<MonsterDefinition>(this, config => config.RawMonsters));
        this.RegisterRepository(new ConfigurationTypeRepository<Skill>(this, config => config.RawSkills));
        this.RegisterRepository(new ConfigurationTypeRepository<PlugInConfiguration>(this, config => config.RawPlugInConfigurations));

        this.RegisterMissingRepositoriesAsGeneric();
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

    /// <inheritdoc/>
    protected override IRepository CreateGenericRepository(Type entityType)
    {
        var repositoryType = typeof(CachingGenericRepository<>).MakeGenericType(entityType);
        return (IRepository)Activator.CreateInstance(repositoryType, this, this._loggerFactory.CreateLogger(repositoryType))!;
    }
}