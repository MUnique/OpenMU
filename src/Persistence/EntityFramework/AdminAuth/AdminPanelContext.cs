// <copyright file="AdminPanelContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.AdminAuth;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>
/// The context which holds the users of the admin panel.
/// </summary>
/// <remarks>
/// It uses an own schema, an own migration history and an own set of migrations.
/// That's on purpose: The admin panel needs its users before the game database exists,
/// because it's the tool which creates the game database in the first place.
/// None of the game server database roles (account, config, guild, friend) gets access
/// to this schema, so a game server process can't read or overwrite an admin password hash.
/// </remarks>
public class AdminPanelContext : DbContext
{
    /// <summary>
    /// Gets or sets the admin panel users.
    /// </summary>
    public DbSet<AdminUser> AdminUsers { get; set; } = null!;

    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        this.Configure(optionsBuilder);

        // The migration history of this context lives in its own schema, so it doesn't
        // interfere with the migrations of the game database.
        optionsBuilder.UseNpgsql(
            ConnectionConfigurator.GetConnectionString<AdminPanelContext>(),
            options => options.MigrationsHistoryTable(HistoryRepository.DefaultTableName, SchemaNames.AdminPanel));
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema(SchemaNames.AdminPanel);
        modelBuilder.Entity<AdminUser>(entity =>
        {
            entity.ToTable(nameof(AdminUser), SchemaNames.AdminPanel);
            entity.HasKey(u => u.Id);
            entity.Property(u => u.LoginName).IsRequired().HasMaxLength(100);
            entity.Property(u => u.NormalizedLoginName).IsRequired().HasMaxLength(100);
            entity.HasIndex(u => u.NormalizedLoginName).IsUnique();
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.SecurityStamp).IsRequired();
            entity.Property(u => u.Roles).IsRequired().HasMaxLength(200);
        });
    }
}
