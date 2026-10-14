// <copyright file="PlayerStoreRestoreTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.PlayerStore;
using MUnique.OpenMU.GameLogic.PlayerShopCurrencies;
using MUnique.OpenMU.GameLogic.Views.PlayerShop;

/// <summary>
/// Tests that a player store which was left open is restored when the player enters the game.
/// </summary>
[TestFixture]
public class PlayerStoreRestoreTest
{
    private const byte StoreSlot = 204;

    /// <summary>
    /// Tests that the item list is sent to the owner when an open store is restored, so that the client shows the prices again.
    /// </summary>
    [Test]
    public async ValueTask RestoredStoreSendsItemListToOwnerAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        await this.AddPricedStoreItemAsync(player).ConfigureAwait(false);
        player.SelectedCharacter!.IsStoreOpened = true;
        player.SelectedCharacter.StoreName = "Test";

        await new OpenStoreAction().RestoreAfterEnterWorldAsync(player, true).ConfigureAwait(false);

        Assert.That(player.ShopStorage!.StoreOpen, Is.True);
        Mock.Get(player.ViewPlugIns.GetPlugIn<IShowShopItemListPlugIn>()!).Verify(p => p.ShowShopItemListAsync(player, true), Times.Once);
    }

    /// <summary>
    /// Tests that no item list is sent when the store was closed.
    /// </summary>
    [Test]
    public async ValueTask ClosedStoreSendsNoItemListAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        await this.AddPricedStoreItemAsync(player).ConfigureAwait(false);

        await new OpenStoreAction().RestoreAfterEnterWorldAsync(player, true).ConfigureAwait(false);

        Assert.That(player.ShopStorage!.StoreOpen, Is.False);
        Mock.Get(player.ViewPlugIns.GetPlugIn<IShowShopItemListPlugIn>()!).Verify(p => p.ShowShopItemListAsync(player, true), Times.Never);
    }

    private async ValueTask AddPricedStoreItemAsync(Player player)
    {
        var item = new Persistence.BasicModel.Item
        {
            Definition = new Persistence.BasicModel.ItemDefinition { Number = 1, Group = 0, Width = 1, Height = 1, Durability = 20 },
            ItemSlot = StoreSlot,
            Durability = 20,
        };
        item.SetStorePrice(new ShopPrice(1000));
        await player.ShopStorage!.AddItemAsync(StoreSlot, item).ConfigureAwait(false);
    }
}
