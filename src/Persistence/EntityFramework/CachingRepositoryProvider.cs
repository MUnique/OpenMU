// <copyright file="CachingRepositoryProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Persistence.EntityFramework.Json;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// A repository provider which utilizes repositories which use the entity framework core to do data access.
/// </summary>
/// <remarks>
/// We create the most repositories by the following convention:
///   - Configuration data repositories: We create repositories which retrieve data from the current GameConfiguration to save memory and less database queries.
///   - Entity data (Accounts, etc.): We create repositories which retrieves every object at every access from the database.
/// </remarks>
internal class CachingRepositoryProvider : RepositoryProvider
{
    private readonly IContextAwareRepositoryProvider _parent;

    /// <summary>
    /// Initializes a new instance of the <see cref="CachingRepositoryProvider" /> class.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="parent">The parent context aware repository provider.</param>
    public CachingRepositoryProvider(ILoggerFactory loggerFactory, IContextAwareRepositoryProvider parent)
        : base(loggerFactory, null, parent.ContextStack)
    {
        this._parent = parent;
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
    /// Registers the repositories.
    /// </summary>
    protected override void Initialize()
    {
        this.RegisterRepository(new CachedRepository<GameConfiguration>(new CachingGameConfigurationRepository(this._parent, this.LoggerFactory)));
        this.RegisterRepository(new CachingGenericRepository<GameServerConfiguration>(this._parent, this.LoggerFactory));
        this.RegisterRepository(new CachingGenericRepository<GameClientDefinition>(this._parent, this.LoggerFactory));
        this.RegisterRepository(new CachingGenericRepository<ConnectServerDefinition>(this._parent, this.LoggerFactory));
        this.RegisterRepository(new CachingGenericRepository<ChatServerDefinition>(this._parent, this.LoggerFactory));
        this.RegisterRepository(new CachingGenericRepository<ChatServerEndpoint>(this._parent, this.LoggerFactory));
        this.RegisterRepository(new CachingGenericRepository<GameServerEndpoint>(this._parent, this.LoggerFactory));
        this.RegisterRepository(new GameServerDefinitionRepository(this._parent, this.LoggerFactory));

        this.RegisterRepository(new ConfigurationTypeRepository<ItemOptionDefinition>(this._parent, this.LoggerFactory, config => config.RawItemOptions));
        this.RegisterRepository(new ConfigurationTypeRepository<IncreasableItemOption>(
            this._parent,
            this.LoggerFactory,
            config => config.RawItemOptions.SelectMany(o => o.RawPossibleOptions).Distinct().ToList()));
        this.RegisterRepository(new ConfigurationTypeRepository<AttributeDefinition>(this._parent, this.LoggerFactory, config => config.RawAttributes));
        this.RegisterRepository(new ConfigurationTypeRepository<DropItemGroup>(this._parent, this.LoggerFactory, config => config.RawDropItemGroups));
        this.RegisterRepository(new ConfigurationTypeRepository<CharacterClass>(this._parent, this.LoggerFactory, config => config.RawCharacterClasses));
        this.RegisterRepository(new ConfigurationTypeRepository<ItemOptionType>(this._parent, this.LoggerFactory, config => config.RawItemOptionTypes));
        this.RegisterRepository(new ConfigurationTypeRepository<ItemSetGroup>(this._parent, this.LoggerFactory, config => config.RawItemSetGroups));
        this.RegisterRepository(new ConfigurationTypeRepository<ItemOfItemSet>(this._parent, this.LoggerFactory, config => config.RawItemSetGroups.SelectMany(g => g.RawItems).ToList()));
        this.RegisterRepository(new ConfigurationTypeRepository<ItemSlotType>(this._parent, this.LoggerFactory, config => config.RawItemSlotTypes));
        this.RegisterRepository(new ConfigurationTypeRepository<ItemDefinition>(this._parent, this.LoggerFactory, config => config.RawItems));
        this.RegisterRepository(new ConfigurationTypeRepository<JewelMix>(this._parent, this.LoggerFactory, config => config.RawJewelMixes));
        this.RegisterRepository(new ConfigurationTypeRepository<MagicEffectDefinition>(this._parent, this.LoggerFactory, config => config.RawMagicEffects));
        this.RegisterRepository(new ConfigurationTypeRepository<GameMapDefinition>(this._parent, this.LoggerFactory, config => config.RawMaps));
        this.RegisterRepository(new ConfigurationTypeRepository<MasterSkillRoot>(this._parent, this.LoggerFactory, config => config.RawMasterSkillRoots));
        this.RegisterRepository(new ConfigurationTypeRepository<MonsterDefinition>(this._parent, this.LoggerFactory, config => config.RawMonsters));
        this.RegisterRepository(new ConfigurationTypeRepository<Skill>(this._parent, this.LoggerFactory, config => config.RawSkills));
        this.RegisterRepository(new ConfigurationTypeRepository<PlugInConfiguration>(this._parent, this.LoggerFactory, config => config.RawPlugInConfigurations));
        this.RegisterRepository(new ConfigurationTypeRepository<QuestDefinition>(this._parent, this.LoggerFactory, config => config.RawMonsters.SelectMany(m => m.RawQuests).ToList()));
        this.RegisterRepository(new ConfigurationTypeRepository<Item>(this._parent, this.LoggerFactory, config => config.RawMonsters.SelectMany(m => m.RawMerchantStore?.RawItems ?? Enumerable.Empty<Item>()).ToList()));

        base.Initialize();
    }

    /// <inheritdoc/>
    protected override IRepository CreateGenericRepository(Type entityType, IContextAwareRepositoryProvider repositoryProvider)
    {
        var repositoryType = typeof(CachingGenericRepository<>).MakeGenericType(entityType);
        return (IRepository)Activator.CreateInstance(repositoryType, this._parent, this.LoggerFactory)!;
    }
}