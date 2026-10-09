// <copyright file="EntityDataContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.Persistence.EntityFramework.Extensions;
using MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// Context for all types of the data model.
/// </summary>
public class EntityDataContext : ExtendedTypeContext
{
    private static readonly Lazy<IModel> CompleteModelValue = new(
        () =>
        {
            using var context = new EntityDataContext();
            return context.Model;
        },
        LazyThreadSafetyMode.PublicationOnly);

    /// <summary>
    /// Gets the model of the <see cref="EntityDataContext"/>, which includes all entity types.
    /// </summary>
    /// <remarks>
    /// The model is the same for every instance, so it's created once instead of instantiating
    /// a throwaway context every time a complete meta model is needed.
    /// </remarks>
    internal static IModel CompleteModel => CompleteModelValue.Value;

    /// <summary>
    /// Gets or sets the current game configuration.
    /// This is used by the <see cref="ConfigurationTypeRepository{T}"/> which gets its data from the current game configuration.
    /// </summary>
    internal GameConfiguration? CurrentGameConfiguration { get; set; }

    /// <summary>
    /// Gets the persistent Castle Siege state.
    /// </summary>
    internal DbSet<CastleSiegeData> CastleSiegeData => this.Set<CastleSiegeData>();

    /// <summary>
    /// Gets the Castle Siege guild registrations.
    /// </summary>
    internal DbSet<CastleSiegeGuildRegistration> CastleSiegeGuildRegistrations => this.Set<CastleSiegeGuildRegistration>();

    /// <summary>
    /// Gets the selected Castle Siege guilds.
    /// </summary>
    internal DbSet<CastleSiegeGuild> CastleSiegeGuilds => this.Set<CastleSiegeGuild>();

    /// <summary>
    /// Gets pending Castle Siege participant rewards.
    /// </summary>
    internal DbSet<CastleSiegePendingReward> CastleSiegePendingRewards => this.Set<CastleSiegePendingReward>();

    /// <summary>
    /// Gets the items of the cash shop storages of the accounts.
    /// </summary>
    internal DbSet<CashShopStorageItem> CashShopStorageItems => this.Set<CashShopStorageItem>();

    /// <summary>
    /// Gets the grants of cash shop coins to the accounts.
    /// </summary>
    internal DbSet<CashShopCoinGrant> CashShopCoinGrants => this.Set<CashShopCoinGrant>();

    /// <summary>
    /// Gets the links of the accounts to users of external services.
    /// </summary>
    internal DbSet<AccountExternalLink> AccountExternalLinks => this.Set<AccountExternalLink>();

    /// <summary>
    /// Gets the bindings of the chats of guilds to channels of external services.
    /// </summary>
    internal DbSet<GuildChatBinding> GuildChatBindings => this.Set<GuildChatBinding>();

    /// <summary>
    /// Gets the gens memberships of the characters.
    /// </summary>
    internal DbSet<GensMember> GensMembers => this.Set<GensMember>();

    /// <summary>
    /// Gets the counts of the recent kills between gens members.
    /// </summary>
    internal DbSet<GensAbuse> GensAbuses => this.Set<GensAbuse>();

    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!ConnectionConfigurator.IsInitialized)
        {
            ConnectionConfigurator.Initialize(new ConfigFileDatabaseConnectionStringProvider());
        }

        base.OnConfiguring(optionsBuilder);
        this.Configure(optionsBuilder);
    }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppearanceData>(o => o.Ignore(p => p.CharacterStatus)); // todo
        modelBuilder.Ignore<ConstantElement>();
        modelBuilder.Ignore<SimpleElement>();
        modelBuilder.Entity<Model.AttributeDefinition>();
        modelBuilder.Entity<ConnectServerDefinition>();
        modelBuilder.Entity<ChatServerDefinition>();
        modelBuilder.Entity<MiniGameRankingEntry>().Apply();
        modelBuilder.Entity<GameServerDefinition>(entity =>
        {
            entity.Property(e => e.PvpEnabled).HasDefaultValue(true);
        });
        modelBuilder.Entity<ConfigurationUpdate>().Apply();
        modelBuilder.Entity<ConfigurationUpdateState>();
        modelBuilder.Entity<SystemConfiguration>();

        modelBuilder.Entity<PowerUpDefinitionValue>().Apply();
        modelBuilder.Entity<Model.ConstValueAttribute>();
        modelBuilder.Entity<Account>().Apply();
        modelBuilder.Entity<Character>().Apply();
        modelBuilder.Entity<CharacterClass>().Apply();
        modelBuilder.Entity<CastleSiegeConfiguration>().Apply();
        modelBuilder.Entity<CastleSiegeData>().Apply();
        modelBuilder.Entity<CastleSiegeGuild>().Apply();
        modelBuilder.Entity<CastleSiegeGuildRegistration>().Apply();
        modelBuilder.Entity<CastleSiegePendingReward>().Apply();
        modelBuilder.Entity<CastleSiegeNpcDefinition>().Apply();
        modelBuilder.Entity<CastleSiegeNpcState>().Apply();
        modelBuilder.Entity<CashShopStorageItem>().Apply();
        modelBuilder.Entity<CashShopCoinGrant>().Apply();
        modelBuilder.Entity<AccountExternalLink>().Apply();
        modelBuilder.Entity<GuildChatBinding>().Apply();
        modelBuilder.Entity<DropItemGroup>().Apply();
        modelBuilder.Entity<ExitGate>().Apply();
        modelBuilder.Entity<GameConfiguration>().Apply();
        modelBuilder.Entity<GameMapDefinition>().Apply();
        modelBuilder.Entity<GensMember>().Apply();
        modelBuilder.Entity<GensAbuse>().Apply();
        modelBuilder.Entity<ItemCrafting>().Apply();
        modelBuilder.Entity<ItemDefinition>().Apply();
        modelBuilder.Entity<ItemLevelBonusTable>().Apply();
        modelBuilder.Entity<ItemDropItemGroup>().Apply();
        modelBuilder.Entity<ItemOptionCombinationBonus>().Apply();
        modelBuilder.Entity<ItemOptionDefinition>().Apply();
        modelBuilder.Entity<ItemOptionType>().Apply();
        modelBuilder.Entity<ItemSetGroup>().Apply();
        modelBuilder.Entity<ItemSlotType>().Apply();
        modelBuilder.Entity<ItemStorage>().Apply();
        modelBuilder.Entity<ItemBasePowerUpDefinition>();
        modelBuilder.Entity<LevelBonus>().Apply();
        modelBuilder.Entity<MagicEffectDefinition>().Apply();
        modelBuilder.Entity<MasterSkillRoot>().Apply();
        modelBuilder.Entity<MiniGameChangeEvent>().Apply();
        modelBuilder.Entity<MiniGameDefinition>().Apply();
        modelBuilder.Entity<MiniGameSpawnWave>().Apply();
        modelBuilder.Entity<MonsterDefinition>().Apply();
        modelBuilder.Entity<MonsterSpawnArea>().Apply();
        modelBuilder.Entity<Skill>().Apply();
        modelBuilder.Entity<SkillComboDefinition>().Apply();
        modelBuilder.Entity<SkillEntry>().Apply();
        modelBuilder.Entity<MasterSkillDefinition>().Apply();
        modelBuilder.Entity<LetterBody>().Apply();
        modelBuilder.Entity<LetterHeader>().Apply();
        modelBuilder.Entity<QuestDefinition>().Apply();
        modelBuilder.Entity<WarpInfo>().Apply();

        // join entity keys:
        this.AddJoinDefinitions(modelBuilder);

        modelBuilder.UseGuidV7Ids();

        GuildContext.ConfigureModel(modelBuilder);
        FriendContext.ConfigureModel(modelBuilder);
    }
}
