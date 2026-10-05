// <copyright file="ConfigurationUpdateVersioningTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests that configuration updates are versioned: an update whose code version
/// is higher than the installed version is offered again, and applying it
/// updates the existing entry instead of adding a duplicate.
/// </summary>
[TestFixture]
internal class ConfigurationUpdateVersioningTest
{
    private const string TestInitializationKey = "test";

    /// <summary>
    /// Resets the code version of the test update before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        VersionedTestUpdatePlugIn.CodeVersion = 1;
    }

    /// <summary>
    /// Tests that an update which was never installed is offered.
    /// </summary>
    [Test]
    public async Task NotInstalledUpdateIsOfferedAsync()
    {
        var service = CreateService();

        var pending = await service.DetermineAvailableUpdatesAsync().ConfigureAwait(false);

        Assert.That(pending.Select(up => up.Key), Does.Contain(VersionedTestUpdatePlugIn.TestKey));
    }

    /// <summary>
    /// Tests that an update which is installed with the current version is not offered again.
    /// </summary>
    [Test]
    public async Task InstalledCurrentVersionIsNotOfferedAsync()
    {
        var (provider, manager) = CreateProviderAndManager();
        AddInstalledEntry(provider, VersionedTestUpdatePlugIn.TestKey, 1);
        var service = new DataUpdateService(provider, manager);

        var pending = await service.DetermineAvailableUpdatesAsync().ConfigureAwait(false);

        Assert.That(pending.Select(up => up.Key), Does.Not.Contain(VersionedTestUpdatePlugIn.TestKey));
    }

    /// <summary>
    /// Tests that an update which is installed with a previous version is offered again.
    /// </summary>
    [Test]
    public async Task InstalledPreviousVersionIsOfferedAgainAsync()
    {
        var (provider, manager) = CreateProviderAndManager();
        AddInstalledEntry(provider, VersionedTestUpdatePlugIn.TestKey, 1);
        VersionedTestUpdatePlugIn.CodeVersion = 2;
        var service = new DataUpdateService(provider, manager);

        var pending = await service.DetermineAvailableUpdatesAsync().ConfigureAwait(false);

        Assert.That(pending.Select(up => up.Key), Does.Contain(VersionedTestUpdatePlugIn.TestKey));
    }

    /// <summary>
    /// Tests that applying an update twice, with a version increase in between,
    /// updates the existing entry instead of adding a duplicate.
    /// </summary>
    [Test]
    public async Task ApplyUpdateUpdatesEntryInPlaceAsync()
    {
        var provider = new InMemoryPersistenceContextProvider();
        using var context = provider.CreateNewContext();
        var gameConfiguration = context.CreateNew<GameConfiguration>();
        var update = new VersionedTestUpdatePlugIn();

        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        VersionedTestUpdatePlugIn.CodeVersion = 2;
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        var entries = (await context.GetAsync<ConfigurationUpdate>().ConfigureAwait(false))
            .Where(entry => entry.Key == VersionedTestUpdatePlugIn.TestKey)
            .ToList();
        Assert.That(entries, Has.Count.EqualTo(1));
        Assert.That(entries[0].Version, Is.EqualTo(VersionedTestUpdatePlugIn.CodeVersion));
        Assert.That(entries[0].InstalledAt, Is.Not.Null);
    }

    /// <summary>
    /// Tests that applying updates backfills the initialization key
    /// when the update state row is missing.
    /// </summary>
    [Test]
    public async Task ApplyUpdatesBackfillsInitializationKeyAsync()
    {
        var (provider, manager) = CreateProviderAndManager();
        using (var context = provider.CreateNewContext())
        {
            _ = context.CreateNew<GameConfiguration>();
        }

        var service = new DataUpdateService(provider, manager);
        var progress = new Progress<(Guid CurrentUpdatingKey, bool IsCompleted)>();
        await service.ApplyUpdatesAsync(new IConfigurationUpdatePlugIn[] { new VersionedTestUpdatePlugIn() }, progress).ConfigureAwait(false);

        using var verifyContext = provider.CreateNewContext();
        var states = await verifyContext.GetAsync<ConfigurationUpdateState>().ConfigureAwait(false);
        Assert.That(states.Select(state => state.InitializationKey), Is.EqualTo(new[] { TestInitializationKey }));
    }

    /// <summary>
    /// Tests that a freshly initialized database marks updates with their current code version.
    /// </summary>
    [Test]
    public async Task FreshDatabaseMarksCurrentVersionAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, false).ConfigureAwait(false);

        using var context = contextProvider.CreateNewContext();
        var entries = await context.GetAsync<ConfigurationUpdate>().ConfigureAwait(false);
        var entry = entries.FirstOrDefault(e => e.Key == typeof(AddDarkHorseCanFlyPlugIn).GUID);

        Assert.That(entry, Is.Not.Null);
        Assert.That(entry?.Version, Is.EqualTo(new AddDarkHorseCanFlyPlugIn().Version));
    }

    private static (InMemoryPersistenceContextProvider Provider, PlugInManager Manager) CreateProviderAndManager()
    {
        var provider = new InMemoryPersistenceContextProvider();
        var manager = new PlugInManager(null, new NullLoggerFactory(), null, null);
        manager.RegisterPlugInAtPlugInPoint<IConfigurationUpdatePlugIn>(new VersionedTestUpdatePlugIn());
        return (provider, manager);
    }

    private static DataUpdateService CreateService()
    {
        var (provider, manager) = CreateProviderAndManager();
        using (var context = provider.CreateNewContext())
        {
            var state = context.CreateNew<ConfigurationUpdateState>();
            state.InitializationKey = TestInitializationKey;
        }

        return new DataUpdateService(provider, manager);
    }

    private static void AddInstalledEntry(InMemoryPersistenceContextProvider provider, Guid key, int version)
    {
        using var context = provider.CreateNewContext();
        var state = context.CreateNew<ConfigurationUpdateState>();
        state.InitializationKey = TestInitializationKey;
        var entry = context.CreateNew<ConfigurationUpdate>();
        entry.Key = key;
        entry.Version = version;
        entry.Name = "Test";
        entry.Description = "Test";
        entry.CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        entry.InstalledAt = DateTime.UtcNow;
    }

    /// <summary>
    /// A test update with a controllable code version.
    /// </summary>
    [Guid("E4A3B2C1-D6E5-4A7B-8C9D-E0F1A2B3C4D5")]
    private sealed class VersionedTestUpdatePlugIn : UpdatePlugInBase
    {
        /// <summary>
        /// Gets the key of the test update.
        /// </summary>
        public static Guid TestKey => typeof(VersionedTestUpdatePlugIn).GUID;

        /// <summary>
        /// Gets or sets the code version of the test update.
        /// </summary>
        public static int CodeVersion { get; set; } = 1;

        /// <inheritdoc />
        public override string DataInitializationKey => TestInitializationKey;

        /// <inheritdoc />
        public override string Name => "Versioned test update";

        /// <inheritdoc />
        public override string Description => "A test update with a controllable version.";

        /// <inheritdoc />
        public override bool IsMandatory => false;

        /// <inheritdoc />
        public override DateTime CreatedAt => new (2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <inheritdoc />
        public override int Version => CodeVersion;

        /// <inheritdoc />
        protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
        {
            return ValueTask.CompletedTask;
        }
    }
}
