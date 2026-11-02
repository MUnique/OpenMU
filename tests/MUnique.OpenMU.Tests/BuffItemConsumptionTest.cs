// <copyright file="BuffItemConsumptionTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests that the buff items of the season 6 data apply their effects when a player uses them.
/// </summary>
[TestFixture]
public class BuffItemConsumptionTest
{
    private const int ItemSlot = 12;

    private GameConfiguration _gameConfiguration = null!;

    /// <summary>
    /// Creates the season 6 data once for all tests.
    /// </summary>
    [OneTimeSetUp]
    public async Task SetUpAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new Persistence.Initialization.VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, false).ConfigureAwait(false);
        using var context = contextProvider.CreateNewContext();
        this._gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
    }

    /// <summary>
    /// Tests that the max SD boost aura increases the maximum shield by ten per character level.
    /// </summary>
    [Test]
    public async ValueTask MaxShieldBoostAuraAddsTenShieldPerLevelAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Attributes![Stats.Level] = 100;
        var shieldBefore = player.Attributes[Stats.MaximumShield];

        var success = await this.ConsumeAsync(player, 13, 105).ConfigureAwait(false);

        Assert.That(success, Is.True);
        Assert.That(player.Attributes[Stats.MaximumShield], Is.EqualTo(shieldBefore + 1000));
    }

    /// <summary>
    /// Tests that the elixir of strength adds 50 strength.
    /// </summary>
    [Test]
    public async ValueTask ElixirOfStrengthAddsStrengthAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var strengthBefore = player.Attributes![Stats.TotalStrength];

        var success = await this.ConsumeAsync(player, 14, 78).ConfigureAwait(false);

        Assert.That(success, Is.True);
        Assert.That(player.Attributes[Stats.TotalStrength], Is.EqualTo(strengthBefore + 50));
    }

    /// <summary>
    /// Tests that a seal replaces the effect of another seal, so that their experience rates don't add up.
    /// </summary>
    [Test]
    public async ValueTask SealReplacesTheEffectOfAnotherSealAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);

        await this.ConsumeAsync(player, 13, 43).ConfigureAwait(false); // Seal of Ascension
        var success = await this.ConsumeAsync(player, 13, 44).ConfigureAwait(false); // Seal of Wealth

        Assert.That(success, Is.True);
        var activeSeals = player.MagicEffectList.GetActiveEffectsSnapshot().Where(e => e.Definition.SubType == 24).ToList();
        Assert.That(activeSeals.Select(e => e.Definition.Number), Is.EqualTo(new short[] { 0x1E }));
    }

    private async ValueTask<bool> ConsumeAsync(Player player, byte group, short number)
    {
        var item = new Item
        {
            Definition = this._gameConfiguration.Items.Single(i => i.Group == group && i.Number == number),
            Durability = 1,
        };

        await player.Inventory!.AddItemAsync(ItemSlot, item).ConfigureAwait(false);
        return await new ApplyMagicEffectConsumeHandlerPlugIn().ConsumeItemAsync(player, item, null, FruitUsage.Undefined).ConfigureAwait(false);
    }
}
