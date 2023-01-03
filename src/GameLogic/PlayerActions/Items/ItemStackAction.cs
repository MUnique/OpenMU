// <copyright file="ItemStackAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items;

using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Action to stack items.
/// </summary>
public class ItemStackAction
{
    /// <summary>
    /// Stacks several items to one stacked item.
    /// </summary>
    /// <param name="player">The player which is stacking.</param>
    /// <param name="stackId">The id of the stacking.</param>
    /// <param name="stackSize">The size of the requested stack.</param>
    public async ValueTask StackItemsAsync(Player player, byte stackId, byte stackSize)
    {
        using var loggerScope = player.Logger.BeginScope(this.GetType());
        if (!this.IsCorrectNpcOpened(player))
        {
            return;
        }

        var mix = this.GetJewelMix(stackId, player);
        if (mix is null)
        {
            return;
        }

        if (player.Inventory is null)
        {
            return;
        }

        var jewels = player.Inventory.Items.Where(item => item.Definition == mix.SingleJewel).Take(stackSize).ToList();
        if (jewels.Count == stackSize)
        {
            foreach (var jewel in jewels)
            {
                await player.Inventory.RemoveItemAsync(jewel).ConfigureAwait(false);
                await player.InvokeViewPlugInAsync<IItemRemovedPlugIn>(p => p.RemoveItemAsync(jewel.ItemSlot)).ConfigureAwait(false);
            }

            var stacked = player.PersistenceContext.CreateNew<Item>();
            stacked.Definition = mix.MixedJewel;
            stacked.Level = (byte)((stackSize / 10) - 1);
            stacked.Durability = 1;
            await player.Inventory.AddItemAsync(stacked).ConfigureAwait(false);
            await player.InvokeViewPlugInAsync<IItemAppearPlugIn>(p => p.ItemAppearAsync(stacked)).ConfigureAwait(false);
        }
        else
        {
            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync("You are lacking of Jewels.", MessageType.BlueNormal)).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Unstacks the stacked item from the specified slot.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="stackId">The stack identifier.</param>
    /// <param name="slot">The slot.</param>
    public async ValueTask UnstackItemsAsync(Player player, byte stackId, byte slot)
    {
        if (!this.IsCorrectNpcOpened(player))
        {
            await player.DisconnectAsync().ConfigureAwait(false);
            return;
        }

        var mix = this.GetJewelMix(stackId, player);
        if (mix is null)
        {
            return;
        }

        var stacked = player.Inventory?.GetItem(slot);
        if (stacked is null)
        {
            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync("Stacked Jewel not found.", MessageType.BlueNormal)).ConfigureAwait(false);
            return;
        }

        if (stacked.Definition != mix.MixedJewel)
        {
            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync("Selected Item is not a stacked Jewel.", MessageType.BlueNormal)).ConfigureAwait(false);
            return;
        }

        byte pieces = (byte)((stacked.Level + 1) * 10);

        var freeSlots = player.Inventory!.FreeSlots.Take(pieces).ToList();
        if (freeSlots.Count < pieces)
        {
            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync("Inventory got not enough Space.", MessageType.BlueNormal)).ConfigureAwait(false);
            return;
        }

        await player.Inventory.RemoveItemAsync(stacked).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<IItemRemovedPlugIn>(p => p.RemoveItemAsync(slot)).ConfigureAwait(false);
        foreach (var freeSlot in freeSlots)
        {
            var jewel = player.PersistenceContext.CreateNew<Item>();
            jewel.Definition = mix.SingleJewel;
            jewel.Durability = 1;
            jewel.ItemSlot = freeSlot;
            await player.Inventory.AddItemAsync(freeSlot, jewel).ConfigureAwait(false);
            await player.InvokeViewPlugInAsync<IItemAppearPlugIn>(p => p.ItemAppearAsync(jewel)).ConfigureAwait(false);
        }
    }

    private bool IsCorrectNpcOpened(Player player)
    {
        if (player.OpenedNpc is null || player.OpenedNpc.Definition.NpcWindow != NpcWindow.Lahap)
        {
            player.Logger.LogWarning("Probably Hacker tried to Mix/Unmix Jewels without talking to Lahap. Dupe Method. Acc: [{0}] Character: [{1}]", player.Account?.LoginName, player.SelectedCharacter?.Name);
            return false;
        }

        return true;
    }

    private JewelMix? GetJewelMix(byte mixId, Player player)
    {
        var mix = player.GameContext.Configuration.JewelMixes.FirstOrDefault(m => m.Number == mixId);
        if (mix is null)
        {
            player.Logger.LogWarning($"Unkown mix type [{mixId}], Player Name: [{player.SelectedCharacter?.Name}], Account Name: [{player.Account?.LoginName}]");
        }

        return mix;
    }
}