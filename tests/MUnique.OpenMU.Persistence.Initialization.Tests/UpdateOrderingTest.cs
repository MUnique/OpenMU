// <copyright file="UpdateOrderingTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Updates;

/// <summary>
/// Tests the dependency ordering of configuration updates.
/// </summary>
[TestFixture]
internal class UpdateOrderingTest
{
    /// <summary>
    /// Tests that an update which depends on another update is applied after it,
    /// regardless of the input order.
    /// </summary>
    [Test]
    public void DependencyIsAppliedFirst()
    {
        var first = new StubUpdatePlugIn(new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc));
        var second = new StubUpdatePlugIn(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), dependsOn: new[] { first.Key });

        var ordered = DataUpdateService.OrderByDependencies(new IConfigurationUpdatePlugIn[] { second, first }, new HashSet<Guid>());

        Assert.That(ordered.Select(up => up.Key), Is.EqualTo(new[] { first.Key, second.Key }));
    }

    /// <summary>
    /// Tests that independent updates are ordered by their creation date.
    /// </summary>
    [Test]
    public void IndependentUpdatesAreOrderedByCreationDate()
    {
        var newer = new StubUpdatePlugIn(new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc));
        var older = new StubUpdatePlugIn(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));

        var ordered = DataUpdateService.OrderByDependencies(new IConfigurationUpdatePlugIn[] { newer, older }, new HashSet<Guid>());

        Assert.That(ordered.Select(up => up.Key), Is.EqualTo(new[] { older.Key, newer.Key }));
    }

    /// <summary>
    /// Tests that independent updates are ordered by their last update date,
    /// so that a version bump is applied after newer updates.
    /// </summary>
    [Test]
    public void IndependentUpdatesAreOrderedByUpdatedAt()
    {
        var createdLater = new StubUpdatePlugIn(new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc));
        var updatedLater = new StubUpdatePlugIn(
            new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc));

        var ordered = DataUpdateService.OrderByDependencies(new IConfigurationUpdatePlugIn[] { updatedLater, createdLater }, new HashSet<Guid>());

        Assert.That(ordered.Select(up => up.Key), Is.EqualTo(new[] { createdLater.Key, updatedLater.Key }));
    }

    /// <summary>
    /// Tests that an already installed dependency doesn't need to be part of the batch.
    /// </summary>
    [Test]
    public void InstalledDependencyIsAccepted()
    {
        var installedKey = Guid.NewGuid();
        var update = new StubUpdatePlugIn(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), dependsOn: new[] { installedKey });

        var ordered = DataUpdateService.OrderByDependencies(new IConfigurationUpdatePlugIn[] { update }, new HashSet<Guid> { installedKey });

        Assert.That(ordered.Select(up => up.Key), Is.EqualTo(new[] { update.Key }));
    }

    /// <summary>
    /// Tests that a dependency which is neither installed nor part of the batch is reported.
    /// </summary>
    [Test]
    public void MissingDependencyThrows()
    {
        var missingKey = Guid.NewGuid();
        var update = new StubUpdatePlugIn(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), dependsOn: new[] { missingKey });

        Assert.Throws<MissingUpdateDependencyException>(() => DataUpdateService.OrderByDependencies(new IConfigurationUpdatePlugIn[] { update }, new HashSet<Guid>()));
    }

    /// <summary>
    /// Tests that a circular dependency between updates is reported.
    /// </summary>
    [Test]
    public void CircularDependencyThrows()
    {
        var first = new StubUpdatePlugIn(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        var second = new StubUpdatePlugIn(new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc), dependsOn: new[] { first.Key });
        first.DependsOn = new[] { second.Key };

        Assert.Throws<CircularUpdateDependencyException>(() => DataUpdateService.OrderByDependencies(new IConfigurationUpdatePlugIn[] { first, second }, new HashSet<Guid>()));
    }

    /// <summary>
    /// A configurable update stub with a unique key.
    /// </summary>
    private sealed class StubUpdatePlugIn : IConfigurationUpdatePlugIn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StubUpdatePlugIn"/> class.
        /// </summary>
        /// <param name="createdAt">The creation date.</param>
        /// <param name="updatedAt">The date of the last change, if different from the creation date.</param>
        /// <param name="dependsOn">The keys of the updates this stub depends on.</param>
        public StubUpdatePlugIn(DateTime createdAt, DateTime? updatedAt = null, params Guid[] dependsOn)
        {
            this.CreatedAt = createdAt;
            this.UpdatedAt = updatedAt ?? createdAt;
            this.DependsOn = dependsOn;
        }

        /// <inheritdoc />
        public Guid Key { get; } = Guid.NewGuid();

        /// <inheritdoc />
        public string DataInitializationKey => "test";

        /// <inheritdoc />
        public string Name => this.Key.ToString();

        /// <inheritdoc />
        public string Description => string.Empty;

        /// <inheritdoc />
        public bool IsMandatory => false;

        /// <inheritdoc />
        public DateTime CreatedAt { get; }

        /// <inheritdoc />
        public DateTime UpdatedAt { get; }

        /// <inheritdoc />
        public int Version => 1;

        /// <inheritdoc />
        public IEnumerable<Guid> DependsOn { get; set; }

        /// <inheritdoc />
        public ValueTask ApplyUpdateAsync(IContext context, GameConfiguration gameConfiguration) => ValueTask.CompletedTask;
    }
}
