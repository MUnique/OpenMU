// <copyright file="JewelerRaulStoreTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests that the Jeweler Raul (NPC 546) in the Loren Market is initialized
/// with a merchant store and a merchant window, so talking to him opens his shop.
/// </summary>
[TestFixture]
internal class JewelerRaulStoreTest
{
    /// <summary>
    /// Tests that Raul is a merchant with a non-empty store which contains
    /// the Season 6 jewels and unique crafting materials.
    /// </summary>
    [Test]
    public async Task JewelerRaulHasMerchantStoreWithJewelsAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        var raul = gameConfiguration.Monsters.First(m => m.Number == 546);
        Assert.That(raul.NpcWindow, Is.EqualTo(NpcWindow.Merchant));
        Assert.That(raul.MerchantStore, Is.Not.Null);
        Assert.That(raul.MerchantStore!.Items, Is.Not.Empty);

        var bless = raul.MerchantStore.Items.Select(i => (Group: i.Definition!.Group, Number: i.Definition.Number, Name: i.Definition.Name.ValueInNeutralLanguage))
            .First(t => t.Group == 14 && t.Number == 13);
        Assert.That(bless.Name, Is.EqualTo("Jewel of Bless"));

        Assert.That(raul.MerchantStore.Items.Any(i => i.Definition!.Group == 14 && i.Definition.Number == 14), Is.True, "Jewel of Soul missing");
        Assert.That(raul.MerchantStore.Items.Any(i => i.Definition!.Group == 12 && i.Definition.Number == 15), Is.True, "Jewel of Chaos missing");
        Assert.That(raul.MerchantStore.Items.Any(i => i.Definition!.Group == 13 && i.Definition.Number == 33), Is.True, "Bless of Guardian missing");
        Assert.That(raul.MerchantStore.Items.All(i => i.Durability == 1), Is.True, "Jewels don't stack, so every store entry must have durability 1");
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
