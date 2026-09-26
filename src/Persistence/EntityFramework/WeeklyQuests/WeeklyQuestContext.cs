// <copyright file="WeeklyQuestContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.WeeklyQuests;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using MUnique.OpenMU.Persistence.WeeklyQuests;

/// <summary>
/// The context which holds the progress of the weekly quests.
/// </summary>
/// <remarks>
/// Like the <see cref="AdminAuth.AdminPanelContext"/>, it uses an own schema, an own migration history
/// and an own set of migrations. This keeps the generated game data model untouched, so the
/// feature doesn't conflict with changes of the upstream project.
/// </remarks>
public class WeeklyQuestContext : DbContext
{
    /// <summary>
    /// Gets or sets the progress entries.
    /// </summary>
    public DbSet<WeeklyQuestProgress> Progress { get; set; } = null!;

    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        this.Configure(optionsBuilder);

        optionsBuilder.UseNpgsql(
            ConnectionConfigurator.GetConnectionString<WeeklyQuestContext>(),
            options => options.MigrationsHistoryTable(HistoryRepository.DefaultTableName, SchemaNames.WeeklyQuests));
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema(SchemaNames.WeeklyQuests);
        modelBuilder.Entity<WeeklyQuestProgress>(entity =>
        {
            entity.ToTable(nameof(WeeklyQuestProgress), SchemaNames.WeeklyQuests);
            entity.HasKey(p => new { p.CharacterId, p.PeriodStart, p.QuestId });
            entity.Property(p => p.QuestId).IsRequired().HasMaxLength(64);
            entity.HasIndex(p => p.PeriodStart);
            entity.HasIndex(p => new { p.AccountId, p.PeriodStart, p.QuestId });
        });
    }
}
