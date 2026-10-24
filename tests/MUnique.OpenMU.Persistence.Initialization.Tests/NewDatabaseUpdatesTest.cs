// <copyright file="NewDatabaseUpdatesTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests the updates on a new database (<see cref="NewDatabaseUpdates"/>): the ones whose changes the initialization
/// contains are only marked as installed; the ones which run on a new database are applied.
/// </summary>
[TestFixture]
internal class NewDatabaseUpdatesTest
{
    /// <summary>
    /// An update of OpenMU itself is marked as installed without being applied.
    /// </summary>
    [Test]
    public async Task UpdateOfTheInitializationIsOnlyMarkedAsync()
    {
        var (context, gameConfiguration) = CreateContext();
        var update = new InitializedTestUpdatePlugIn();

        await NewDatabaseUpdates.InstallAsync(context, gameConfiguration, [update]).ConfigureAwait(false);

        Assert.That(MarkerNames(gameConfiguration), Does.Not.Contain(update.Name));
        Assert.That(await InstalledVersionAsync(context, update.Key).ConfigureAwait(false), Is.EqualTo(update.Version));
    }

    /// <summary>
    /// An update which runs on a new database, e.g. a plugin's which adds its own item, is applied and installed.
    /// </summary>
    [Test]
    public async Task UpdateWhichRunsOnNewDatabaseIsAppliedAsync()
    {
        var (context, gameConfiguration) = CreateContext();
        var update = new SeedingTestUpdatePlugIn();

        await NewDatabaseUpdates.InstallAsync(context, gameConfiguration, [update, new InitializedTestUpdatePlugIn()]).ConfigureAwait(false);

        Assert.That(gameConfiguration.Items.Select(item => item.Name.Value), Does.Contain(SeedingTestUpdatePlugIn.ItemName));
        Assert.That(await InstalledVersionAsync(context, update.Key).ConfigureAwait(false), Is.EqualTo(update.Version));
    }

    /// <summary>
    /// The entry of a marked update describes it as an applied one does: its version, name, description and dates.
    /// </summary>
    [Test]
    public async Task MarkedEntryDescribesTheUpdateAsync()
    {
        var (context, gameConfiguration) = CreateContext();
        var update = new InitializedTestUpdatePlugIn();

        await NewDatabaseUpdates.InstallAsync(context, gameConfiguration, [update]).ConfigureAwait(false);

        var entry = (await context.GetAsync<ConfigurationUpdate>().ConfigureAwait(false)).Single(e => e.Key == update.Key);
        Assert.That(entry.Version, Is.EqualTo(update.Version));
        Assert.That(entry.Name.Value, Is.EqualTo(update.Name));
        Assert.That(entry.Description.Value, Is.EqualTo(update.Description));
        Assert.That(entry.CreatedAt, Is.EqualTo(update.CreatedAt));
        Assert.That(entry.UpdatedAt, Is.EqualTo(update.UpdatedAt));
        Assert.That(entry.InstalledAt, Is.Not.Null);
    }

    /// <summary>
    /// The updates which run on a new database are applied in the order of their dates, as the admin panel applies
    /// pending updates, whatever the order they come in; an applied update's entry has its version.
    /// </summary>
    [Test]
    public async Task UpdatesRunInTheOrderOfTheirDatesAsync()
    {
        var (context, gameConfiguration) = CreateContext();
        var earlier = new EarlierTestUpdatePlugIn();
        var later = new LaterTestUpdatePlugIn();

        await NewDatabaseUpdates.InstallAsync(context, gameConfiguration, [later, earlier]).ConfigureAwait(false);

        Assert.That(MarkerNames(gameConfiguration), Is.EqualTo(new[] { earlier.Name, later.Name }));
        Assert.That(await InstalledVersionAsync(context, later.Key).ConfigureAwait(false), Is.EqualTo(3));
    }

    /// <summary>
    /// Without updates, nothing is installed.
    /// </summary>
    [Test]
    public async Task NoUpdatesInstallNothingAsync()
    {
        var (context, gameConfiguration) = CreateContext();

        await NewDatabaseUpdates.InstallAsync(context, gameConfiguration, []).ConfigureAwait(false);

        Assert.That(await context.GetAsync<ConfigurationUpdate>().ConfigureAwait(false), Is.Empty);
    }

    /// <summary>
    /// An update runs after the update it depends on, also when its own date is earlier.
    /// </summary>
    [Test]
    public async Task DependencyRunsFirstWhateverTheDatesAsync()
    {
        var (context, gameConfiguration) = CreateContext();
        var dependency = Configured("dependency", 2);
        var dependent = Configured("dependent", 1, dependsOn: [dependency.Key]);

        await NewDatabaseUpdates.InstallAsync(context, gameConfiguration, [dependent, dependency]).ConfigureAwait(false);

        Assert.That(MarkerNames(gameConfiguration), Is.EqualTo(new[] { "dependency", "dependent" }));
    }

    /// <summary>
    /// An update which depends on an update that isn't there can't be installed.
    /// </summary>
    [Test]
    public void MissingDependencyIsRefused()
    {
        var (context, gameConfiguration) = CreateContext();
        var dependent = Configured("dependent", 1, dependsOn: [Guid.NewGuid()]);

        Assert.ThrowsAsync<MissingUpdateDependencyException>(async () => await NewDatabaseUpdates.InstallAsync(context, gameConfiguration, [dependent]).ConfigureAwait(false));
        Assert.That(MarkerNames(gameConfiguration), Is.Empty);
    }

    /// <summary>
    /// An update which needs a higher version of its dependency than there is can't be installed; with that version,
    /// it is.
    /// </summary>
    [Test]
    public async Task MinimumVersionOfTheDependencyCountsAsync()
    {
        var (context, gameConfiguration) = CreateContext();
        var oldDependency = Configured("dependency", 1);
        var dependent = Configured("dependent", 2, dependsOn: [(oldDependency.Key, 2)]);

        Assert.ThrowsAsync<MissingUpdateDependencyException>(async () => await NewDatabaseUpdates.InstallAsync(context, gameConfiguration, [oldDependency, dependent]).ConfigureAwait(false));

        var newDependency = Configured("dependency", 1, version: 2, key: oldDependency.Key);
        await NewDatabaseUpdates.InstallAsync(context, gameConfiguration, [newDependency, dependent]).ConfigureAwait(false);
        Assert.That(MarkerNames(gameConfiguration), Is.EqualTo(new[] { "dependency", "dependent" }));
    }

    /// <summary>
    /// Updates which depend on each other can't be installed.
    /// </summary>
    [Test]
    public void CircularDependenciesAreRefused()
    {
        var (context, gameConfiguration) = CreateContext();
        var firstKey = Guid.NewGuid();
        var secondKey = Guid.NewGuid();
        var first = Configured("first", 1, key: firstKey, dependsOn: [secondKey]);
        var second = Configured("second", 2, key: secondKey, dependsOn: [firstKey]);

        Assert.ThrowsAsync<CircularUpdateDependencyException>(async () => await NewDatabaseUpdates.InstallAsync(context, gameConfiguration, [first, second]).ConfigureAwait(false));
    }

    /// <summary>
    /// An optional update which would run on a new database stays pending, as on an existing database, so the operator
    /// decides; so does a mandatory one which depends on it.
    /// </summary>
    [Test]
    public async Task OptionalUpdateStaysPendingWithItsDependentsAsync()
    {
        var (context, gameConfiguration) = CreateContext();
        var optional = Configured("optional", 1, isMandatory: false);
        var dependent = Configured("dependent", 2, dependsOn: [optional.Key]);
        var independent = Configured("independent", 3);

        await NewDatabaseUpdates.InstallAsync(context, gameConfiguration, [optional, dependent, independent]).ConfigureAwait(false);

        Assert.That(MarkerNames(gameConfiguration), Is.EqualTo(new[] { "independent" }));
        Assert.That(await InstalledVersionAsync(context, optional.Key).ConfigureAwait(false), Is.Null);
        Assert.That(await InstalledVersionAsync(context, dependent.Key).ConfigureAwait(false), Is.Null);
    }

    /// <summary>
    /// An update whose changes the initialization contains is installed, optional or not, and also when it depends on
    /// an update which stays pending.
    /// </summary>
    [Test]
    public async Task UpdatesTheInitializationContainsAreInstalledAnywayAsync()
    {
        var (context, gameConfiguration) = CreateContext();
        var optionalSeeding = Configured("optional seeding", 1, isMandatory: false);
        var optionalInitialized = Configured("optional initialized", 2, isMandatory: false, runsOnNewDatabase: false);
        var initializedDependent = Configured("initialized dependent", 3, runsOnNewDatabase: false, dependsOn: [optionalSeeding.Key]);

        await NewDatabaseUpdates.InstallAsync(context, gameConfiguration, [optionalSeeding, optionalInitialized, initializedDependent]).ConfigureAwait(false);

        Assert.That(MarkerNames(gameConfiguration), Is.Empty);
        Assert.That(await InstalledVersionAsync(context, optionalInitialized.Key).ConfigureAwait(false), Is.EqualTo(1));
        Assert.That(await InstalledVersionAsync(context, initializedDependent.Key).ConfigureAwait(false), Is.EqualTo(1));
        Assert.That(await InstalledVersionAsync(context, optionalSeeding.Key).ConfigureAwait(false), Is.Null);
    }

    /// <summary>
    /// An optional update which stays pending doesn't stop the initialization when its dependency is missing, or comes
    /// in a version that is too low: its dependencies are checked when it's applied. The other updates are installed.
    /// </summary>
    [Test]
    public async Task BrokenOptionalUpdateStaysPendingAsync()
    {
        var (context, gameConfiguration) = CreateContext();
        var dependency = Configured("dependency", 1);
        var missing = Configured("missing dependency", 2, isMandatory: false, dependsOn: [Guid.NewGuid()]);
        var tooLow = Configured("too low dependency", 3, isMandatory: false, dependsOn: [(dependency.Key, 2)]);
        var mandatory = Configured("mandatory", 4);

        await NewDatabaseUpdates.InstallAsync(context, gameConfiguration, [dependency, missing, tooLow, mandatory]).ConfigureAwait(false);

        Assert.That(MarkerNames(gameConfiguration), Is.EqualTo(new[] { "dependency", "mandatory" }));
        Assert.That(await InstalledVersionAsync(context, missing.Key).ConfigureAwait(false), Is.Null);
        Assert.That(await InstalledVersionAsync(context, tooLow.Key).ConfigureAwait(false), Is.Null);
    }

    /// <summary>
    /// Optional updates which stay pending don't stop the initialization when they depend on each other in a circle.
    /// </summary>
    [Test]
    public async Task OptionalUpdatesInACircleStayPendingAsync()
    {
        var (context, gameConfiguration) = CreateContext();
        var firstKey = Guid.NewGuid();
        var secondKey = Guid.NewGuid();
        var first = Configured("first", 1, isMandatory: false, key: firstKey, dependsOn: [secondKey]);
        var second = Configured("second", 2, isMandatory: false, key: secondKey, dependsOn: [firstKey]);
        var mandatory = Configured("mandatory", 3);

        await NewDatabaseUpdates.InstallAsync(context, gameConfiguration, [first, second, mandatory]).ConfigureAwait(false);

        Assert.That(MarkerNames(gameConfiguration), Is.EqualTo(new[] { "mandatory" }));
    }

    /// <summary>
    /// An update to apply can depend on one whose changes the initialization contains: that one counts as installed.
    /// </summary>
    [Test]
    public async Task UpdateToApplyCanDependOnAnInitializedOneAsync()
    {
        var (context, gameConfiguration) = CreateContext();
        var initialized = Configured("initialized", 1, version: 2, runsOnNewDatabase: false);
        var dependent = Configured("dependent", 2, dependsOn: [(initialized.Key, 2)]);

        await NewDatabaseUpdates.InstallAsync(context, gameConfiguration, [dependent, initialized]).ConfigureAwait(false);

        Assert.That(MarkerNames(gameConfiguration), Is.EqualTo(new[] { "dependent" }));
        Assert.That(await InstalledVersionAsync(context, initialized.Key).ConfigureAwait(false), Is.EqualTo(2));
    }

    /// <summary>
    /// Adds the marker item of an applied test update: an item named like the update.
    /// </summary>
    private static ValueTask AddMarkerAsync(IContext context, GameConfiguration gameConfiguration, string name)
    {
        var marker = context.CreateNew<ItemDefinition>();
        marker.Name = name;
        gameConfiguration.Items.Add(marker);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// An update for the dependency tests, by default a mandatory one which runs on a new database, dated on the given
    /// day of April 2026.
    /// </summary>
    private static ConfiguredTestUpdatePlugIn Configured(string name, int day, int version = 1, bool isMandatory = true, bool runsOnNewDatabase = true, Guid? key = null, UpdateDependency[]? dependsOn = null)
    {
        return new ConfiguredTestUpdatePlugIn(key ?? Guid.NewGuid(), name, new DateTime(2026, 4, day, 0, 0, 0, DateTimeKind.Utc), version, isMandatory, runsOnNewDatabase, dependsOn ?? []);
    }

    private static List<string?> MarkerNames(GameConfiguration gameConfiguration)
    {
        return gameConfiguration.Items.Select(item => item.Name.Value).ToList();
    }

    private static (IContext Context, GameConfiguration GameConfiguration) CreateContext()
    {
        var context = new InMemoryPersistenceContextProvider().CreateNewContext();
        return (context, context.CreateNew<GameConfiguration>());
    }

    private static async Task<int?> InstalledVersionAsync(IContext context, Guid key)
    {
        return (await context.GetAsync<ConfigurationUpdate>().ConfigureAwait(false))
            .FirstOrDefault(entry => entry.Key == key && entry.InstalledAt is not null)?.Version;
    }

    /// <summary>
    /// An update whose changes the initialization contains.
    /// </summary>
    [Guid("2B6E9C40-5A1D-4F83-9E07-C14D8A3B6F25")]
    private sealed class InitializedTestUpdatePlugIn : UpdatePlugInBase
    {
        public override string DataInitializationKey => "test";

        public override string Name => "Initialized test update";

        public override string Description => "An update whose changes the initialization contains.";

        public override bool IsMandatory => true;

        public override DateTime CreatedAt => new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
        {
            return AddMarkerAsync(context, gameConfiguration, this.Name);
        }
    }

    /// <summary>
    /// The updates of the order test: they run on a new database and add a marker item when they're applied.
    /// </summary>
    private abstract class OrderedTestUpdatePlugIn : UpdatePlugInBase
    {
        private readonly string _name;
        private readonly DateTime _createdAt;
        private readonly int _version;

        protected OrderedTestUpdatePlugIn(string name, DateTime createdAt, int version)
        {
            this._name = name;
            this._createdAt = createdAt;
            this._version = version;
        }

        public override string DataInitializationKey => "test";

        public override string Name => this._name;

        public override string Description => "An update which adds a marker item when it's applied.";

        public override bool IsMandatory => true;

        public override DateTime CreatedAt => this._createdAt;

        public override int Version => this._version;

        public override bool RunsOnNewDatabase => true;
    }

    /// <summary>
    /// The earlier update of the order test.
    /// </summary>
    [Guid("5E8A2D17-C94B-4F03-B6E1-0A7D3C9F2B48")]
    private sealed class EarlierTestUpdatePlugIn : OrderedTestUpdatePlugIn
    {
        public EarlierTestUpdatePlugIn()
            : base("earlier", new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc), 1)
        {
        }

        protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
        {
            return AddMarkerAsync(context, gameConfiguration, this.Name);
        }
    }

    /// <summary>
    /// The later update of the order test, at version 3.
    /// </summary>
    [Guid("A1C6F3E8-2D74-4B95-8E0F-6B3A9D5C1E27")]
    private sealed class LaterTestUpdatePlugIn : OrderedTestUpdatePlugIn
    {
        public LaterTestUpdatePlugIn()
            : base("later", new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc), 3)
        {
        }

        protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
        {
            return AddMarkerAsync(context, gameConfiguration, this.Name);
        }
    }

    /// <summary>
    /// An update which adds an item the initialization doesn't, as a plugin's would.
    /// </summary>
    [Guid("9D47F1A2-3C68-4B5E-A0D9-71E2B8C40F63")]
    private sealed class SeedingTestUpdatePlugIn : UpdatePlugInBase
    {
        public const string ItemName = "Seeded test item";

        public override string DataInitializationKey => "test";

        public override string Name => "Seeding test update";

        public override string Description => "An update which adds an item the initialization doesn't.";

        public override bool IsMandatory => true;

        public override DateTime CreatedAt => new(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc);

        public override bool RunsOnNewDatabase => true;

        protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
        {
            var item = context.CreateNew<ItemDefinition>();
            item.Name = ItemName;
            gameConfiguration.Items.Add(item);
            return ValueTask.CompletedTask;
        }
    }

    /// <summary>
    /// An update of the dependency tests, with its key, date, version, flags and dependencies given; applied, it adds
    /// a marker item and its installed entry.
    /// </summary>
    private sealed class ConfiguredTestUpdatePlugIn : IConfigurationUpdatePlugIn
    {
        public ConfiguredTestUpdatePlugIn(Guid key, string name, DateTime createdAt, int version, bool isMandatory, bool runsOnNewDatabase, IEnumerable<UpdateDependency> dependsOn)
        {
            this.Key = key;
            this.Name = name;
            this.CreatedAt = createdAt;
            this.Version = version;
            this.IsMandatory = isMandatory;
            this.RunsOnNewDatabase = runsOnNewDatabase;
            this.DependsOn = dependsOn;
        }

        public Guid Key { get; }

        public string DataInitializationKey => "test";

        public string Name { get; }

        public string Description => "An update of the dependency tests.";

        public bool IsMandatory { get; }

        public DateTime CreatedAt { get; }

        public bool RunsOnNewDatabase { get; }

        public int Version { get; }

        public IEnumerable<UpdateDependency> DependsOn { get; }

        public async ValueTask ApplyUpdateAsync(IContext context, GameConfiguration gameConfiguration)
        {
            await AddMarkerAsync(context, gameConfiguration, this.Name).ConfigureAwait(false);
            var entry = context.CreateNew<ConfigurationUpdate>();
            entry.Key = this.Key;
            entry.Version = this.Version;
            entry.Name = this.Name;
            entry.Description = this.Description;
            entry.CreatedAt = this.CreatedAt;
            entry.UpdatedAt = this.CreatedAt;
            entry.InstalledAt = DateTime.UtcNow;
        }
    }
}
