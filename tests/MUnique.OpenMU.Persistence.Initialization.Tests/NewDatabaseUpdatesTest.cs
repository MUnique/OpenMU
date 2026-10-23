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

        Assert.That(update.Applied, Is.False);
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
        var applied = new List<string>();
        var earlier = new OrderedTestUpdatePlugIn(applied, "earlier", new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc), 1);
        var later = new LaterOrderedTestUpdatePlugIn(applied, "later", new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc), 3);

        await NewDatabaseUpdates.InstallAsync(context, gameConfiguration, [later, earlier]).ConfigureAwait(false);

        Assert.That(applied, Is.EqualTo(new[] { "earlier", "later" }));
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
        public bool Applied { get; private set; }

        public override string DataInitializationKey => "test";

        public override string Name => "Initialized test update";

        public override string Description => "An update whose changes the initialization contains.";

        public override bool IsMandatory => true;

        public override DateTime CreatedAt => new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
        {
            this.Applied = true;
            return ValueTask.CompletedTask;
        }
    }

    /// <summary>
    /// An update which runs on a new database and notes when it's applied.
    /// </summary>
    [Guid("5E8A2D17-C94B-4F03-B6E1-0A7D3C9F2B48")]
    private class OrderedTestUpdatePlugIn : UpdatePlugInBase
    {
        private readonly List<string> _applied;
        private readonly string _name;
        private readonly DateTime _createdAt;
        private readonly int _version;

        public OrderedTestUpdatePlugIn(List<string> applied, string name, DateTime createdAt, int version)
        {
            this._applied = applied;
            this._name = name;
            this._createdAt = createdAt;
            this._version = version;
        }

        public override string DataInitializationKey => "test";

        public override string Name => this._name;

        public override string Description => "An update which notes when it's applied.";

        public override bool IsMandatory => true;

        public override DateTime CreatedAt => this._createdAt;

        public override int Version => this._version;

        public override bool RunsOnNewDatabase => true;

        protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
        {
            this._applied.Add(this._name);
            return ValueTask.CompletedTask;
        }
    }

    /// <summary>
    /// A second update of that kind: an update is identified by its type's GUID.
    /// </summary>
    [Guid("A1C6F3E8-2D74-4B95-8E0F-6B3A9D5C1E27")]
    private sealed class LaterOrderedTestUpdatePlugIn : OrderedTestUpdatePlugIn
    {
        public LaterOrderedTestUpdatePlugIn(List<string> applied, string name, DateTime createdAt, int version)
            : base(applied, name, createdAt, version)
        {
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
}
