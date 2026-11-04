// <copyright file="ClearInventoryChatCommandPlugInTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

/// <summary>
/// Tests for the <see cref="ClearInventoryChatCommandPlugIn"/>.
/// </summary>
[TestFixture]
public class ClearInventoryChatCommandPlugInTest
{
    private const string Command = "/clearinv";
    private const byte InventorySlot = 12;

    /// <summary>
    /// Verifies that the first command of a regular player only asks for a confirmation.
    /// </summary>
    [Test]
    public async ValueTask FirstCommandOnlyAsksForConfirmationAsync()
    {
        var (player, item) = await CreatePlayerWithItemAsync().ConfigureAwait(false);
        var plugin = CreatePlugIn();

        await plugin.HandleCommandAsync(player, Command).ConfigureAwait(false);

        Assert.That(player.Inventory!.Items, Does.Contain(item));
    }

    /// <summary>
    /// Verifies that repeating the command confirms it and clears the inventory.
    /// </summary>
    [Test]
    public async ValueTask SecondCommandClearsInventoryAsync()
    {
        var (player, item) = await CreatePlayerWithItemAsync().ConfigureAwait(false);
        var plugin = CreatePlugIn();

        await plugin.HandleCommandAsync(player, Command).ConfigureAwait(false);
        await plugin.HandleCommandAsync(player, Command).ConfigureAwait(false);

        Assert.That(player.Inventory!.Items, Does.Not.Contain(item));
    }

    /// <summary>
    /// Verifies that a confirmation is consumed by the clearing, so the next command asks again.
    /// </summary>
    [Test]
    public async ValueTask ConfirmationIsConsumedByClearingAsync()
    {
        var (player, _) = await CreatePlayerWithItemAsync().ConfigureAwait(false);
        var plugin = CreatePlugIn();
        await plugin.HandleCommandAsync(player, Command).ConfigureAwait(false);
        await plugin.HandleCommandAsync(player, Command).ConfigureAwait(false);

        var newItem = CreateItem();
        await player.Inventory!.AddItemAsync(InventorySlot, newItem).ConfigureAwait(false);
        await plugin.HandleCommandAsync(player, Command).ConfigureAwait(false);

        Assert.That(player.Inventory.Items, Does.Contain(newItem));
    }

    private static ClearInventoryChatCommandPlugIn CreatePlugIn()
    {
        return new ClearInventoryChatCommandPlugIn
        {
            Configuration = new ClearInventoryChatCommandPlugIn.ClearInventoryConfiguration
            {
                RequireConfirmation = true,
            },
        };
    }

    private static async ValueTask<(Player Player, Item Item)> CreatePlayerWithItemAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.SelectedCharacter!.CharacterStatus = CharacterStatus.Normal;
        var item = CreateItem();
        await player.Inventory!.AddItemAsync(InventorySlot, item).ConfigureAwait(false);
        return (player, item);
    }

    private static Item CreateItem()
    {
        return new Item
        {
            Definition = new ItemDefinition { Group = 14, Number = 21, Width = 1, Height = 1 },
            ItemSlot = InventorySlot,
        };
    }
}
