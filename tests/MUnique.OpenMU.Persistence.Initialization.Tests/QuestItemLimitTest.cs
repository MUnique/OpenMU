// <copyright file="QuestItemLimitTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests that the quest item limits and flags are correct in the season 6 data,
/// and that the versioned update restores them on existing databases.
/// </summary>
[TestFixture]
internal class QuestItemLimitTest
{
    /// <summary>
    /// Tests that a new season 6 database limits the quest items and flags them as such.
    /// </summary>
    [Test]
    public async Task NewDatabaseLimitsAndFlagsQuestItemsAsync()
    {
        var (_, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);

        var questItems = GetQuestItems(gameConfiguration);
        Assert.That(questItems, Is.Not.Empty);
        Assert.That(questItems.All(item => item.StorageLimitPerCharacter == 1 && item.IsQuestItem), Is.True);
    }

    /// <summary>
    /// Tests that the update restores the quest item limits and flags on an existing
    /// database, and that applying it twice doesn't change anything.
    /// </summary>
    [Test]
    public async Task UpdateRestoresQuestItemLimitsAndFlagsAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        var questItems = GetQuestItems(gameConfiguration);
        Assert.That(questItems, Is.Not.Empty);
        foreach (var item in questItems)
        {
            item.StorageLimitPerCharacter = 0;
            item.IsQuestItem = false;
        }

        var update = new AddQuestItemLimitPlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(questItems.All(item => item.StorageLimitPerCharacter == 1 && item.IsQuestItem), Is.True);
    }

    private static List<ItemDefinition> GetQuestItems(GameConfiguration gameConfiguration)
    {
        return gameConfiguration.Items.Where(item => item.IsQuestItem && item.StorageLimitPerCharacter == 1).ToList();
    }

    private static async Task<(IContext Context, GameConfiguration GameConfiguration)> CreateSeason6ConfigurationAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);

        var context = contextProvider.CreateNewContext();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
        return (context, gameConfiguration);
    }
}
