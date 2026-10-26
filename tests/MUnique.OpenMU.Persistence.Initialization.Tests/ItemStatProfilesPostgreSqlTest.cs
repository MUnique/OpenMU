// <copyright file="ItemStatProfilesPostgreSqlTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.EntityFramework;
using MUnique.OpenMU.Persistence.EntityFramework.Json;
using MUnique.OpenMU.Persistence.Initialization.Items;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests the item stat profiles with a PostgreSQL database: the migration on a database which has data,
/// the update which adds the profiles, the configuration loader which reads them back, and the unique indexes.
/// Set <c>OPENMU_TEST_POSTGRES</c> to the connection string of a disposable PostgreSQL server, with a user which
/// may create databases, to run it. It creates a database with a unique name and drops it afterwards.
/// </summary>
[TestFixture]
[NonParallelizable]
internal class ItemStatProfilesPostgreSqlTest
{
    private const string MigrationName = "_AddItemStatProfiles";

    private readonly string _databaseName = "openmu_item_stat_profiles_" + Guid.NewGuid().ToString("N");
    private IDisposable? _connectionSettings;
    private PersistenceContextProvider _contextProvider = null!;

    /// <summary>
    /// Creates the database with all migrations and the data of version 0.75, which has the profiles.
    /// </summary>
    [OneTimeSetUp]
    public async Task CreateDatabaseAsync()
    {
        var connectionString = Environment.GetEnvironmentVariable("OPENMU_TEST_POSTGRES");
        if (string.IsNullOrEmpty(connectionString))
        {
            Assert.Ignore("Set OPENMU_TEST_POSTGRES to run the PostgreSQL tests of the item stat profiles.");
        }

        // The migrations read the role names from 'User Id=...;' and their passwords from 'Password=...;'.
        var server = new NpgsqlConnectionStringBuilder(connectionString);
        var databaseConnection = $"Server={server.Host};Port={server.Port};User Id={server.Username};Password={server.Password};Database={this._databaseName};Command Timeout=120;";
        this._connectionSettings = ConnectionConfigurator.UseTemporarily(new OneDatabaseSettingProvider(databaseConnection));

        // The host registers them as well; the configuration loader needs them.
        if (!JsonConverterRegistry.Converters.OfType<LocalizedStringJsonConverter>().Any())
        {
            JsonConverterRegistry.RegisterConverter(new LocalizedStringJsonConverter());
        }

        if (!JsonConverterRegistry.Converters.OfType<BinaryAsHexJsonConverter>().Any())
        {
            JsonConverterRegistry.RegisterConverter(new BinaryAsHexJsonConverter());
        }

        this._contextProvider = new PersistenceContextProvider(new NullLoggerFactory(), null);
        await this._contextProvider.ApplyAllPendingUpdatesAsync().ConfigureAwait(false);
        await new Version075.DataInitialization(this._contextProvider, new NullLoggerFactory()).CreateInitialDataAsync(1, false).ConfigureAwait(false);
    }

    /// <summary>
    /// Drops the database and brings back the previous connection settings.
    /// </summary>
    [OneTimeTearDown]
    public async Task DropDatabaseAsync()
    {
        if (this._connectionSettings is null)
        {
            return;
        }

        try
        {
            await using var context = new EntityDataContext();
            await context.Database.EnsureDeletedAsync().ConfigureAwait(false);
        }
        finally
        {
            this._connectionSettings.Dispose();
        }
    }

    /// <summary>
    /// Tests that the migration keeps the data of a database which didn't have the profiles yet and gives the new
    /// columns their defaults, that the update adds the profiles and gives the items theirs, and that the configuration
    /// loader reads them back with all relationships as a new database has them.
    /// </summary>
    [Test]
    public async Task MigrationUpdateAndLoaderKeepTheProfilesAsync()
    {
        var expected = await this.DescribeAsync().ConfigureAwait(false);

        await this.MigrateBackAndUpAsync().ConfigureAwait(false);

        using (var context = this._contextProvider.CreateNewContext())
        {
            var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();
            Assert.Multiple(() =>
            {
                Assert.That(gameConfiguration.Items, Is.Not.Empty);
                Assert.That(gameConfiguration.ItemStatProfiles, Is.Empty);
                Assert.That(gameConfiguration.Items.Where(item => item.StatProfile is not null), Is.Empty);
                Assert.That(gameConfiguration.Items.Where(item => !item.ExcellentRaisesLevel || item.ExcellentBonus != 0), Is.Empty);
                Assert.That(gameConfiguration.ExcellentDropLevelBonus, Is.EqualTo(25));
                Assert.That(gameConfiguration.AncientDropLevelBonus, Is.EqualTo(30));
            });

            await new AddItemStatProfilesUpdatePlugIn075().ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        Assert.That(await this.DescribeAsync().ConfigureAwait(false), Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests that the migration and the update keep an item which an owner gave another level bonus table than the other
    /// items of its kind: the update completes, no item's table changes, and the item gets a variant of its profile
    /// which names its table.
    /// </summary>
    [Test]
    public async Task MigrationAndUpdateKeepAnItemWithAnotherTableAsync()
    {
        short swordNumber;
        IReadOnlyList<string> powerUpTables;
        using (var context = this._contextProvider.CreateNewContext())
        {
            var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();
            var sword = gameConfiguration.Items
                .Where(item => item.Group == 0 && item.BasePowerUpAttributes.Any(IsDamageWithTable))
                .OrderBy(item => item.Number)
                .First();
            var table = context.CreateNew<ItemLevelBonusTable>();
            table.Name = "Own damage increase of one sword";
            gameConfiguration.ItemLevelBonusTables.Add(table);
            foreach (var powerUp in sword.BasePowerUpAttributes.Where(IsDamageWithTable))
            {
                powerUp.BonusPerLevelTable = table;
            }

            await context.SaveChangesAsync().ConfigureAwait(false);
            swordNumber = sword.Number;
            powerUpTables = ItemStatProfilesDataTest.DescribePowerUpTables(gameConfiguration);
        }

        await this.MigrateBackAndUpAsync().ConfigureAwait(false);
        using (var context = this._contextProvider.CreateNewContext())
        {
            var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();
            await new AddItemStatProfilesUpdatePlugIn075().ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        using (var context = this._contextProvider.CreateNewContext())
        {
            var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();
            var sword = gameConfiguration.Items.Single(item => item.Group == 0 && item.Number == swordNumber);
            Assert.Multiple(() =>
            {
                Assert.That(sword.StatProfile?.Name, Is.EqualTo(BuiltInItemStatProfiles.Weapon + "2"));
                Assert.That(sword.StatProfile?.StatTerms.Single(t => t.Attribute == Stats.MinimumPhysBaseDmgByWeapon).Table?.Name.ValueInNeutralLanguage, Is.EqualTo("Own damage increase of one sword"));
                Assert.That(ItemStatProfilesDataTest.DescribePowerUpTables(gameConfiguration), Is.EqualTo(powerUpTables));
                Assert.That(ItemStatProfilesDataTest.GetTablesWhichDifferFromTheProfile(gameConfiguration), Is.Empty);
            });
        }
    }

    /// <summary>
    /// Tests that the database rejects a second profile with the same name in a configuration, and a second stat terms
    /// and requirement formula of the same stat in a profile.
    /// </summary>
    [Test]
    public async Task UniqueIndexesRejectDuplicatesAsync()
    {
        await this.AssertRejectedAsync((context, gameConfiguration) =>
        {
            AddProfile(context, gameConfiguration, "twice");
            AddProfile(context, gameConfiguration, "twice");
        }).ConfigureAwait(false);

        await this.AssertRejectedAsync((context, gameConfiguration) =>
        {
            var profile = AddProfile(context, gameConfiguration, "terms twice");
            for (var i = 0; i < 2; i++)
            {
                var terms = context.CreateNew<ItemStatTerms>();
                terms.Attribute = Stats.MinimumPhysBaseDmgByWeapon.GetPersistent(gameConfiguration);
                profile.StatTerms.Add(terms);
            }
        }).ConfigureAwait(false);

        await this.AssertRejectedAsync((context, gameConfiguration) =>
        {
            var profile = AddProfile(context, gameConfiguration, "formula twice");
            for (var i = 0; i < 2; i++)
            {
                var formula = context.CreateNew<ItemRequirementFormula>();
                formula.Attribute = Stats.TotalStrengthRequirementValue.GetPersistent(gameConfiguration);
                profile.RequirementFormulas.Add(formula);
            }
        }).ConfigureAwait(false);
    }

    private static bool IsDamageWithTable(ItemBasePowerUpDefinition powerUp)
    {
        return powerUp.BonusPerLevelTable is not null
               && (powerUp.TargetAttribute == Stats.MinimumPhysBaseDmgByWeapon || powerUp.TargetAttribute == Stats.MaximumPhysBaseDmgByWeapon);
    }

    private static ItemStatProfile AddProfile(IContext context, GameConfiguration gameConfiguration, string name)
    {
        var profile = context.CreateNew<ItemStatProfile>();
        profile.Name = name;
        gameConfiguration.ItemStatProfiles.Add(profile);
        return profile;
    }

    /// <summary>
    /// Describes the profiles and the profile of each item as the configuration loader reads them, and checks that the
    /// level bonus table of each stat is one of the configuration.
    /// </summary>
    private async Task<string> DescribeAsync()
    {
        using var context = this._contextProvider.CreateNewContext();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();
        var foreignTables = gameConfiguration.ItemStatProfiles
            .SelectMany(profile => profile.StatTerms.Where(terms => terms.Table is not null && !gameConfiguration.ItemLevelBonusTables.Contains(terms.Table)))
            .Select(terms => $"{terms.Attribute}: {terms.Table!.Name}");
        Assert.That(foreignTables, Is.Empty);
        return ItemStatProfilesDataTest.Describe(gameConfiguration);
    }

    /// <summary>
    /// Brings the database back to the state before the migration, which removes the profiles with their tables and
    /// columns, and applies the migrations again.
    /// </summary>
    private async Task MigrateBackAndUpAsync()
    {
        await using (var context = new EntityDataContext())
        {
            var migrations = context.Database.GetMigrations().ToList();
            var migration = migrations.Single(m => m.EndsWith(MigrationName, StringComparison.Ordinal));
            await context.GetService<IMigrator>().MigrateAsync(migrations[migrations.IndexOf(migration) - 1]).ConfigureAwait(false);
            Assert.That(await context.Database.GetAppliedMigrationsAsync().ConfigureAwait(false), Does.Not.Contain(migration));
        }

        await this._contextProvider.ApplyAllPendingUpdatesAsync().ConfigureAwait(false);
    }

    private async Task AssertRejectedAsync(Action<IContext, GameConfiguration> addDuplicate)
    {
        using var context = this._contextProvider.CreateNewContext();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();
        addDuplicate(context, gameConfiguration);

        // The context reports a rejected change as a PersistenceConflictException around the exception of the database.
        var exception = Assert.ThrowsAsync<PersistenceConflictException>(async () => await context.SaveChangesAsync().ConfigureAwait(false));
        Assert.That(exception!.InnerException?.InnerException, Is.InstanceOf<PostgresException>().With.Property(nameof(PostgresException.SqlState)).EqualTo(PostgresErrorCodes.UniqueViolation));
    }

    /// <summary>
    /// Gives every context the connection to the test database.
    /// </summary>
    private sealed class OneDatabaseSettingProvider : IDatabaseConnectionSettingProvider
    {
        private readonly string _connectionString;

        public OneDatabaseSettingProvider(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public Task? Initialization => null;

        public Task InitializeAsync(CancellationToken cancellationToken)
            => cancellationToken.IsCancellationRequested ? Task.FromCanceled(cancellationToken) : Task.CompletedTask;

        public ConnectionSetting GetConnectionSetting<TContextType>()
            where TContextType : DbContext
            => this.GetConnectionSetting(typeof(TContextType));

        public ConnectionSetting GetConnectionSetting(Type contextType)
            => new() { ContextTypeName = contextType.FullName, ConnectionString = this._connectionString, DatabaseEngine = DatabaseEngine.Npgsql };
    }
}
