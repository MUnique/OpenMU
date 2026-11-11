// <copyright file="GladiatorsHonorDataTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests the data of the Gladiator's Honor and the <see cref="AddGladiatorsHonorUpdatePlugIn"/>.
/// </summary>
[TestFixture]
internal class GladiatorsHonorDataTest
{
    /// <summary>
    /// Tests that the update adds the item, its effect and its attribute to a database which was created before
    /// they existed. Applying it twice doesn't add them twice.
    /// </summary>
    [Test]
    public async Task UpdateAddsItemToExistingDatabaseAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        var item = gameConfiguration.Items.Single(IsGladiatorsHonor);
        gameConfiguration.Items.Remove(item);
        gameConfiguration.MagicEffects.Remove(item.ConsumeEffect!);
        gameConfiguration.Attributes.Remove(gameConfiguration.Attributes.Single(a => a.Id == Stats.VulcanusExperienceRate.Id));

        var update = new AddGladiatorsHonorUpdatePlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        var addedItem = gameConfiguration.Items.Single(IsGladiatorsHonor);
        var powerUp = addedItem.ConsumeEffect!.PowerUpDefinitions.Single();
        Assert.That(gameConfiguration.MagicEffects.Count(e => e.Number == 0x67), Is.EqualTo(1));
        Assert.That(gameConfiguration.Attributes.Count(a => a.Id == Stats.VulcanusExperienceRate.Id), Is.EqualTo(1));
        Assert.That(powerUp.TargetAttribute, Is.SameAs(gameConfiguration.Attributes.Single(a => a.Id == Stats.VulcanusExperienceRate.Id)));
        Assert.That(powerUp.Boost!.ConstantValue.Value, Is.EqualTo(0.2f).Within(0.0001f));
        Assert.That(addedItem.IsTradable, Is.False);
    }

    private static bool IsGladiatorsHonor(ItemDefinition item) => item is { Group: 13, Number: 95 };

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
