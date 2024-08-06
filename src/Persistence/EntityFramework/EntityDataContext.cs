// <copyright file="EntityDataContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

namespace MUnique.OpenMU.Persistence.EntityFramework;

using Microsoft.EntityFrameworkCore;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// Context for all types of the data model.
/// </summary>
public class EntityDataContext : ExtendedTypeContext
{
    /// <summary>
    /// Gets or sets the current game configuration.
    /// This is used by the <see cref="ConfigurationTypeRepository{T}"/> which gets its data from the current game configuration.
    /// </summary>
    internal GameConfiguration? CurrentGameConfiguration { get; set; }

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

        modelBuilder.Ignore<ConstantElement>();
        modelBuilder.Ignore<SimpleElement>();
        modelBuilder.Entity<Model.AttributeDefinition>();
        modelBuilder.Entity<ConnectServerDefinition>();
        modelBuilder.Entity<ChatServerDefinition>();
        modelBuilder.Entity<MiniGameRankingEntry>();
        modelBuilder.Entity<GameServerDefinition>();
        modelBuilder.Entity<ConfigurationUpdate>();
        modelBuilder.Entity<ConfigurationUpdateState>();
        modelBuilder.Entity<SystemConfiguration>();

        modelBuilder.Entity<PowerUpDefinitionValue>().Apply();
        modelBuilder.Entity<Model.ConstValueAttribute>().Apply();
        modelBuilder.Entity<Account>().Apply();
        modelBuilder.Entity<Character>().Apply();
        modelBuilder.Entity<ItemStorage>().Apply();
        modelBuilder.Entity<ItemSetGroup>().Apply();
        modelBuilder.Entity<ItemBasePowerUpDefinition>().Apply();
        modelBuilder.Entity<LevelBonus>().Apply();
        modelBuilder.Entity<ExitGate>().Apply();
        modelBuilder.Entity<GameMapDefinition>().Apply();
        modelBuilder.Entity<MonsterSpawnArea>().Apply();
        modelBuilder.Entity<SkillEntry>().Apply();
        modelBuilder.Entity<CharacterClass>().Apply();
        modelBuilder.Entity<MasterSkillDefinition>().Apply();
        modelBuilder.Entity<LetterBody>().Apply();
        modelBuilder.Entity<LetterHeader>().Apply();
        modelBuilder.Entity<MonsterDefinition>().Apply();
        modelBuilder.Entity<GameConfiguration>().Apply();

        // join entity keys:
        this.AddJoinDefinitions(modelBuilder);

        var types = modelBuilder.Model.GetEntityTypes();
        foreach (var t in types)
        {
            var entity = modelBuilder.Entity(t.ClrType);
            var key = entity.Metadata.FindProperty("Id");
            if (key != null)
            {
                key.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAdd;
                key.SetValueGeneratorFactory((_, _) => new GuidV7ValueGenerator());
            }
        }

        GuildContext.ConfigureModel(modelBuilder);
        FriendContext.ConfigureModel(modelBuilder);
    }
}