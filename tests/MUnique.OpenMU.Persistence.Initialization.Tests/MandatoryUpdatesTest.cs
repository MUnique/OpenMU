// <copyright file="MandatoryUpdatesTest.cs" company="MUnique">
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
/// Tests the mandatory updates of an existing database (<see cref="DataUpdateService.DetermineMandatoryUpdatesAsync"/>),
/// which the all-in-one server applies at start, and that applying them installs them.
/// </summary>
[TestFixture]
internal class MandatoryUpdatesTest
{
    private const string TestInitializationKey = "test";

    /// <summary>
    /// Only the mandatory updates are determined; the optional ones stay for the admin panel.
    /// </summary>
    [Test]
    public async Task OptionalUpdatesAreLeftOutAsync()
    {
        var mandatory = Update("mandatory", 1);
        var optional = Update("optional", 2, isMandatory: false);
        var service = CreateService(new InMemoryPersistenceContextProvider(), mandatory, optional);

        var updates = await service.DetermineMandatoryUpdatesAsync().ConfigureAwait(false);

        Assert.That(updates.Select(update => update.Name), Is.EqualTo(new[] { "mandatory" }));
    }

    /// <summary>
    /// A mandatory update which is installed with its current version is not determined again.
    /// </summary>
    [Test]
    public async Task InstalledUpdateIsLeftOutAsync()
    {
        var provider = new InMemoryPersistenceContextProvider();
        var installed = Update("installed", 1);
        AddInstalledEntry(provider, installed.Key, 1);
        var service = CreateService(provider, installed);

        var updates = await service.DetermineMandatoryUpdatesAsync().ConfigureAwait(false);

        Assert.That(updates, Is.Empty);
    }

    /// <summary>
    /// A mandatory update which is installed with a previous version is determined again.
    /// </summary>
    [Test]
    public async Task UpdateInstalledInAPreviousVersionIsDeterminedAsync()
    {
        var provider = new InMemoryPersistenceContextProvider();
        var extended = Update("extended", 1, version: 2);
        AddInstalledEntry(provider, extended.Key, 1);
        var service = CreateService(provider, extended);

        var updates = await service.DetermineMandatoryUpdatesAsync().ConfigureAwait(false);

        Assert.That(updates.Select(update => update.Name), Is.EqualTo(new[] { "extended" }));
    }

    /// <summary>
    /// A mandatory update of another data initialization than the one of the database is not determined.
    /// </summary>
    [Test]
    public async Task UpdateOfAnotherInitializationIsLeftOutAsync()
    {
        var other = Update("other", 1, initializationKey: "other");
        var service = CreateService(new InMemoryPersistenceContextProvider(), other);

        var updates = await service.DetermineMandatoryUpdatesAsync().ConfigureAwait(false);

        Assert.That(updates, Is.Empty);
    }

    /// <summary>
    /// The mandatory updates come in the order in which they are applied: an update after the update it depends on,
    /// also when its own date is earlier.
    /// </summary>
    [Test]
    public async Task UpdatesAreOrderedByTheirDependenciesAsync()
    {
        var dependency = Update("dependency", 2);
        var dependent = Update("dependent", 1, dependsOn: [dependency.Key]);
        var service = CreateService(new InMemoryPersistenceContextProvider(), dependent, dependency);

        var updates = await service.DetermineMandatoryUpdatesAsync().ConfigureAwait(false);

        Assert.That(updates.Select(update => update.Name), Is.EqualTo(new[] { "dependency", "dependent" }));
    }

    /// <summary>
    /// A mandatory update which depends on an optional one which isn't installed is left out, so the operator decides
    /// about both on the admin panel; so is a mandatory update which depends on it.
    /// </summary>
    [Test]
    public async Task UpdateWhichDependsOnAnOptionalOneIsLeftOutAsync()
    {
        var optional = Update("optional", 1, isMandatory: false);
        var dependent = Update("dependent", 2, dependsOn: [optional.Key]);
        var indirectDependent = Update("indirect dependent", 3, dependsOn: [dependent.Key]);
        var independent = Update("independent", 4);
        var service = CreateService(new InMemoryPersistenceContextProvider(), optional, dependent, indirectDependent, independent);

        var updates = await service.DetermineMandatoryUpdatesAsync().ConfigureAwait(false);

        Assert.That(updates.Select(update => update.Name), Is.EqualTo(new[] { "independent" }));
    }

    /// <summary>
    /// A mandatory update which depends on an installed optional one is determined, also when the optional one is
    /// available in a newer version, as long as the dependency doesn't require that version.
    /// </summary>
    [Test]
    public async Task UpdateWhichDependsOnAnInstalledOptionalOneIsDeterminedAsync()
    {
        var provider = new InMemoryPersistenceContextProvider();
        var optional = Update("optional", 1, version: 2, isMandatory: false);
        AddInstalledEntry(provider, optional.Key, 1);
        var dependent = Update("dependent", 2, dependsOn: [optional.Key]);
        var requiresNewerVersion = Update("requires newer version", 3, dependsOn: [(optional.Key, 2)]);
        var service = CreateService(provider, optional, dependent, requiresNewerVersion);

        var updates = await service.DetermineMandatoryUpdatesAsync().ConfigureAwait(false);

        Assert.That(updates.Select(update => update.Name), Is.EqualTo(new[] { "dependent" }));
    }

    /// <summary>
    /// An optional update which depends on an update which isn't available (e.g. a disabled one) doesn't prevent
    /// anything at start: without a mandatory update, nothing is determined, and a mandatory update is still determined.
    /// The admin panel still reports the missing dependency.
    /// </summary>
    [Test]
    public async Task OptionalUpdateWithAMissingDependencyDoesNotPreventAnythingAsync()
    {
        var optional = Update("optional", 1, isMandatory: false, dependsOn: [Guid.NewGuid()]);
        var withoutMandatory = CreateService(new InMemoryPersistenceContextProvider(), optional);
        var withMandatory = CreateService(new InMemoryPersistenceContextProvider(), optional, Update("mandatory", 2));

        Assert.That(await withoutMandatory.DetermineMandatoryUpdatesAsync().ConfigureAwait(false), Is.Empty);
        Assert.That((await withMandatory.DetermineMandatoryUpdatesAsync().ConfigureAwait(false)).Select(update => update.Name), Is.EqualTo(new[] { "mandatory" }));
        Assert.That(async () => await withoutMandatory.DetermineAvailableUpdatesAsync().ConfigureAwait(false), Throws.TypeOf<MissingUpdateDependencyException>());
    }

    /// <summary>
    /// A mandatory update which depends on an update which isn't available (e.g. a disabled one) is left out instead
    /// of preventing the start; so is a mandatory update which depends on it.
    /// </summary>
    [Test]
    public async Task UpdateWhichDependsOnAMissingOneIsLeftOutAsync()
    {
        var dependent = Update("dependent", 1, dependsOn: [Guid.NewGuid()]);
        var indirectDependent = Update("indirect dependent", 2, dependsOn: [dependent.Key]);
        var independent = Update("independent", 3);
        var service = CreateService(new InMemoryPersistenceContextProvider(), dependent, indirectDependent, independent);

        var updates = await service.DetermineMandatoryUpdatesAsync().ConfigureAwait(false);

        Assert.That(updates.Select(update => update.Name), Is.EqualTo(new[] { "independent" }));
    }

    /// <summary>
    /// Applying the determined updates applies them in their order and marks them installed with their version, so
    /// no mandatory update is left afterwards, while the optional ones are still available for the admin panel.
    /// </summary>
    [Test]
    public async Task AppliedUpdatesAreInstalledAsync()
    {
        var provider = new InMemoryPersistenceContextProvider();
        var applied = new List<string>();
        var dependency = Update("dependency", 2, version: 3, applied: applied);
        var dependent = Update("dependent", 1, dependsOn: [dependency.Key], applied: applied);
        var optional = Update("optional", 3, isMandatory: false, applied: applied);
        var service = CreateService(provider, dependent, dependency, optional);

        var updates = await service.DetermineMandatoryUpdatesAsync().ConfigureAwait(false);
        await service.ApplyUpdatesAsync(updates, new Progress<(Guid CurrentUpdatingKey, bool IsCompleted)>()).ConfigureAwait(false);

        Assert.That(applied, Is.EqualTo(new[] { "dependency", "dependent" }));
        Assert.That(await InstalledVersionAsync(provider, dependency.Key).ConfigureAwait(false), Is.EqualTo(3));
        Assert.That(await InstalledVersionAsync(provider, dependent.Key).ConfigureAwait(false), Is.EqualTo(1));
        Assert.That(await service.DetermineMandatoryUpdatesAsync().ConfigureAwait(false), Is.Empty);
        Assert.That((await service.DetermineAvailableUpdatesAsync().ConfigureAwait(false)).Select(update => update.Name), Is.EqualTo(new[] { "optional" }));
    }

    /// <summary>
    /// An update of these tests, by default a mandatory one of the test initialization, dated on the given day of
    /// April 2026.
    /// </summary>
    private static TestUpdatePlugIn Update(string name, int day, int version = 1, bool isMandatory = true, string initializationKey = TestInitializationKey, UpdateDependency[]? dependsOn = null, List<string>? applied = null)
    {
        return new TestUpdatePlugIn(Guid.NewGuid(), name, initializationKey, new DateTime(2026, 4, day, 0, 0, 0, DateTimeKind.Utc), version, isMandatory, dependsOn ?? [], applied ?? []);
    }

    /// <summary>
    /// Creates the update service for a database of the test initialization with a game configuration, which knows
    /// the given updates.
    /// </summary>
    private static DataUpdateService CreateService(InMemoryPersistenceContextProvider provider, params TestUpdatePlugIn[] updates)
    {
        using (var context = provider.CreateNewContext())
        {
            _ = context.CreateNew<GameConfiguration>();
            var state = context.CreateNew<ConfigurationUpdateState>();
            state.InitializationKey = TestInitializationKey;
        }

        var manager = new PlugInManager(null, new NullLoggerFactory(), null, null);
        foreach (var update in updates)
        {
            manager.RegisterPlugInAtPlugInPoint<IConfigurationUpdatePlugIn>(update);
        }

        return new DataUpdateService(provider, manager);
    }

    private static void AddInstalledEntry(InMemoryPersistenceContextProvider provider, Guid key, int version)
    {
        using var context = provider.CreateNewContext();
        var entry = context.CreateNew<ConfigurationUpdate>();
        entry.Key = key;
        entry.Version = version;
        entry.Name = "Installed";
        entry.Description = "An installed update.";
        entry.CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        entry.InstalledAt = DateTime.UtcNow;
    }

    private static async Task<int?> InstalledVersionAsync(InMemoryPersistenceContextProvider provider, Guid key)
    {
        using var context = provider.CreateNewContext();
        return (await context.GetAsync<ConfigurationUpdate>().ConfigureAwait(false))
            .FirstOrDefault(entry => entry.Key == key && entry.InstalledAt is not null)?.Version;
    }

    /// <summary>
    /// An update of these tests, with its key, initialization, date, version, flag and dependencies given; applied, it
    /// adds its name to the list of applied updates and adds or updates its installed entry.
    /// </summary>
    [Guid("36A3A5B5-D77A-417E-8BD6-AD21C2916140")]
    private sealed class TestUpdatePlugIn : IConfigurationUpdatePlugIn
    {
        private readonly List<string> _applied;

        public TestUpdatePlugIn(Guid key, string name, string dataInitializationKey, DateTime createdAt, int version, bool isMandatory, IEnumerable<UpdateDependency> dependsOn, List<string> applied)
        {
            this.Key = key;
            this.Name = name;
            this.DataInitializationKey = dataInitializationKey;
            this.CreatedAt = createdAt;
            this.Version = version;
            this.IsMandatory = isMandatory;
            this.DependsOn = dependsOn;
            this._applied = applied;
        }

        public Guid Key { get; }

        public string DataInitializationKey { get; }

        public string Name { get; }

        public string Description => "An update of the mandatory update tests.";

        public bool IsMandatory { get; }

        public DateTime CreatedAt { get; }

        public int Version { get; }

        public IEnumerable<UpdateDependency> DependsOn { get; }

        public async ValueTask ApplyUpdateAsync(IContext context, GameConfiguration gameConfiguration)
        {
            // The update service hands over the configuration it loaded, as the real updates need it.
            ArgumentNullException.ThrowIfNull(gameConfiguration);
            this._applied.Add(this.Name);
            var entry = (await context.GetAsync<ConfigurationUpdate>().ConfigureAwait(false)).FirstOrDefault(e => e.Key == this.Key)
                        ?? context.CreateNew<ConfigurationUpdate>();
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
