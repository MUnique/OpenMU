// <copyright file="StandardizeEventEnterDurationTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Updates;

/// <summary>
/// Tests that the update raises the entrance duration to the standardized five minutes, keeps owner tweaks and per party events, and is a no-op on a second run.
/// </summary>
[TestFixture]
internal class StandardizeEventEnterDurationTest
{
    /// <summary>
    /// Tests that a freshly initialized database already announces the standardized
    /// entrance duration, so the update has nothing left to do.
    /// </summary>
    [Test]
    public async Task FreshDatabaseHasEnterDurationAsync()
    {
        var (context, gameConfiguration) = await TestGameConfiguration.CreateSeason6Async().ConfigureAwait(false);
        using var _ = context;

        var shared = GetSharedDefinitions(gameConfiguration);
        Assert.That(shared, Is.Not.Empty);
        Assert.That(shared.Select(d => d.EnterDuration), Is.All.EqualTo(MiniGameConstants.EnterDuration));
    }

    /// <summary>
    /// Tests that the update raises the seeded one and three minute entrance durations to
    /// the standardized five minutes, and that applying it again changes nothing.
    /// </summary>
    [Test]
    public async Task UpdateRaisesSeededEnterDurationsAsync()
    {
        var (context, gameConfiguration) = await TestGameConfiguration.CreateSeason6Async().ConfigureAwait(false);
        using var _ = context;

        var shared = GetSharedDefinitions(gameConfiguration);
        Assert.That(shared, Is.Not.Empty);
        foreach (var definition in shared)
        {
            definition.EnterDuration = TimeSpan.FromMinutes(1);
        }

        // Kanturu was seeded with three minutes instead of one.
        gameConfiguration.MiniGameDefinitions.First(d => d.Type == MiniGameType.Kanturu).EnterDuration = TimeSpan.FromMinutes(3);

        var update = new StandardizeEventEnterDurationPlugInSeason6();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        Assert.That(shared.Select(d => d.EnterDuration), Is.All.EqualTo(MiniGameConstants.EnterDuration));

        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        Assert.That(shared.Select(d => d.EnterDuration), Is.All.EqualTo(MiniGameConstants.EnterDuration));
    }

    /// <summary>
    /// Tests that an entrance duration which the owner has deliberately set to a value the
    /// initializer never seeded is left alone.
    /// </summary>
    [Test]
    public async Task UpdateKeepsCustomizedEnterDurationAsync()
    {
        var (context, gameConfiguration) = await TestGameConfiguration.CreateSeason6Async().ConfigureAwait(false);
        using var _ = context;

        var definition = GetSharedDefinitions(gameConfiguration).First();
        definition.EnterDuration = TimeSpan.FromMinutes(2);

        var update = new StandardizeEventEnterDurationPlugInSeason6();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(definition.EnterDuration, Is.EqualTo(TimeSpan.FromMinutes(2)));
    }

    /// <summary>
    /// Tests that the events which are created per party keep their entrance duration, as
    /// they have no server-wide entrance announcement which could be standardized.
    /// </summary>
    [Test]
    public async Task UpdateKeepsEnterDurationOfPerPartyEventsAsync()
    {
        var (context, gameConfiguration) = await TestGameConfiguration.CreateSeason6Async().ConfigureAwait(false);
        using var _ = context;

        var perParty = gameConfiguration.MiniGameDefinitions
            .Where(d => d.MapCreationPolicy == MiniGameMapCreationPolicy.OnePerParty)
            .ToList();
        Assert.That(perParty, Is.Not.Empty);
        var formerDurations = perParty.Select(d => d.EnterDuration).ToList();

        var update = new StandardizeEventEnterDurationPlugInSeason6();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(perParty.Select(d => d.EnterDuration), Is.EqualTo(formerDurations));
    }

    /// <summary>
    /// Tests that the update also runs against a 0.95d database, which is where Devil Square was introduced.
    /// </summary>
    [Test]
    public async Task UpdateRaisesSeededEnterDurationOn095dAsync()
    {
        var (context, gameConfiguration) = await TestGameConfiguration.Create095dAsync().ConfigureAwait(false);
        using var _ = context;

        var shared = GetSharedDefinitions(gameConfiguration);
        Assert.That(shared, Is.Not.Empty);
        foreach (var definition in shared)
        {
            definition.EnterDuration = TimeSpan.FromMinutes(1);
        }

        var update = new StandardizeEventEnterDurationPlugIn095d();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(shared.Select(d => d.EnterDuration), Is.All.EqualTo(MiniGameConstants.EnterDuration));
    }

    private static List<MiniGameDefinition> GetSharedDefinitions(GameConfiguration gameConfiguration)
    {
        return gameConfiguration.MiniGameDefinitions
            .Where(d => d.MapCreationPolicy == MiniGameMapCreationPolicy.Shared)
            .ToList();
    }
}
