// <copyright file="EntityDataContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

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

        modelBuilder.Entity<PowerUpDefinitionValue>().Ignore(p => p.ConstantValue);
        modelBuilder.Entity<Model.ConstValueAttribute>().Ignore(c => c.AggregateType);

        modelBuilder.Entity<Account>(entity =>
        {
            entity.Property(account => account.LoginName).HasMaxLength(10).IsRequired();
            entity.HasIndex(account => account.LoginName).IsUnique();
        });

        modelBuilder.Entity<Character>(entity =>
        {
            entity.Property(character => character.Name).HasMaxLength(10).IsRequired();
            entity.HasIndex(character => character.Name).IsUnique();
            entity.Metadata.FindNavigation(nameof(Character.RawCharacterClass))!.ForeignKey.IsRequired = true;
            entity.Property(character => character.CharacterSlot).IsRequired();
            var accountKey = entity.Metadata.GetForeignKeys().First(key => key.PrincipalEntityType == modelBuilder.Entity<Account>().Metadata);
            accountKey.DeleteBehavior = DeleteBehavior.Cascade;

            entity.HasMany(character => character.RawLetters).WithOne(letter => letter.Receiver!).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ItemStorage>().HasMany(storage => storage.RawItems).WithOne(item => item.RawItemStorage!);
        modelBuilder.Entity<GameServerDefinition>();
        modelBuilder.Entity<ItemBasePowerUpDefinition>().Ignore(d => d.BaseValueElement);
        modelBuilder.Entity<LevelBonus>().Ignore(l => l.AdditionalValueElement);
        modelBuilder.Entity<ExitGate>().HasOne(gate => gate.RawMap);
        modelBuilder.Entity<GameMapDefinition>().HasMany(map => map.RawEnterGates);
        modelBuilder.Entity<GameMapDefinition>().HasMany(map => map.RawExitGates).WithOne(g => g.RawMap);
        modelBuilder.Entity<GameMapDefinition>().HasOne(map => map.RawSafezoneMap);
        modelBuilder.Entity<GameMapDefinition>().HasMany(map => map.RawMonsterSpawns);

        modelBuilder.Entity<MonsterSpawnArea>().HasOne(spawn => spawn.RawMonsterDefinition);
        modelBuilder.Entity<MonsterSpawnArea>().HasOne(spawn => spawn.RawGameMap);

        modelBuilder.Entity<SkillEntry>().Ignore(s => s.PowerUps);
        modelBuilder.Entity<SkillEntry>().Ignore(s => s.PowerUpDuration);
        modelBuilder.Entity<Model.ConstValueAttribute>().Ignore(c => c.AggregateType);
        modelBuilder.Entity<CharacterClass>()
            .HasMany(c => c.RawBaseAttributeValues)
            .WithOne(c => c.CharacterClass!);
        modelBuilder.Entity<Model.StatAttribute>().Ignore("ValueGetter");

        modelBuilder.Entity<MasterSkillDefinition>().HasOne(s => s.RawRoot);
        modelBuilder.Entity<LetterBody>().HasOne(body => body.RawHeader);
        modelBuilder.Entity<LetterHeader>().Ignore(header => header.ReceiverName);
        modelBuilder.Entity<MonsterDefinition>().HasMany<QuestDefinition>().WithOne(q => q.RawQuestGiver);
        modelBuilder.Entity<MiniGameRankingEntry>();

        // TODO:
        modelBuilder.Entity<GameConfiguration>().Ignore(c => c.ExperienceTable)
            .Ignore(c => c.MasterExperienceTable);

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
            }
        }

        GuildContext.ConfigureModel(modelBuilder);
        FriendContext.ConfigureModel(modelBuilder);
    }
}