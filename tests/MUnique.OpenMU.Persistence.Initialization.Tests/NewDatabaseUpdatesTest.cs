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
