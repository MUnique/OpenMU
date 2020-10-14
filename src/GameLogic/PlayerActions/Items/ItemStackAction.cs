// <copyright file="ItemStackAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items
{
    using System.Linq;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
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
        public void StackItems(Player player, byte stackId, byte stackSize)
        {
            using var loggerScope = player.Logger.BeginScope(this.GetType());
            if (!this.IsCorrectNpcOpened(player))
            {
                return;
            }

            var mix = this.GetJewelMix(stackId, player);
            if (mix == null)
            {
                return;
            }

            var jewels = player.Inventory.Items.Where(item => item.Definition == mix.SingleJewel).Take(stackSize).ToList();
            if (jewels.Count == stackSize)
            {
                foreach (Item jewel in jewels)
                {
                    player.Inventory.RemoveItem(jewel);
                    player.ViewPlugIns.GetPlugIn<IItemRemovedPlugIn>()?.RemoveItem(jewel.ItemSlot);
                }

                var stacked = player.PersistenceContext.CreateNew<Item>();
                stacked.Definition = mix.MixedJewel;
                stacked.Level = (byte)(stackSize / 10);
                stacked.Durability = 1;
                player.Inventory.AddItem(stacked);
                player.ViewPlugIns.GetPlugIn<IItemAppearPlugIn>()?.ItemAppear(stacked);
            }
            else
            {
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("You are lacking of Jewels.", MessageType.BlueNormal);
            }
        }

        /// <summary>
        /// Unstacks the stacked item from the specified slot.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="stackId">The stack identifier.</param>
        /// <param name="slot">The slot.</param>
        public void UnstackItems(Player player, byte stackId, byte slot)
        {
            if (!this.IsCorrectNpcOpened(player))
            {
                return;
            }

            var mix = this.GetJewelMix(stackId, player);
            if (mix == null)
            {
                return;
            }

            var stacked = player.Inventory.GetItem(slot);
            if (stacked == null)
            {
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("Stacked Jewel not found.", MessageType.BlueNormal);
                return;
            }

            if (stacked.Definition != mix.SingleJewel)
            {
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("Selected Item is not a stacked Jewel.", MessageType.BlueNormal);
                return;
            }

            byte pieces = (byte)((stacked.Level + 1) * 10);

            var freeSlots = player.Inventory.FreeSlots.Take(pieces).ToList();
            if (freeSlots.Count < pieces)
            {
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("Inventory got not enough Space.", MessageType.BlueNormal);
                return;
            }

            player.Inventory.RemoveItem(stacked);
            player.ViewPlugIns.GetPlugIn<IItemRemovedPlugIn>()?.RemoveItem(slot);
            foreach (var freeSlot in freeSlots)
            {
                var jewel = player.PersistenceContext.CreateNew<Item>();
                jewel.Definition = mix.SingleJewel;
                jewel.Durability = 1;
                jewel.ItemSlot = freeSlot;
                player.Inventory.AddItem(freeSlot, jewel);
                player.ViewPlugIns.GetPlugIn<IItemAppearPlugIn>()?.ItemAppear(jewel);
            }
        }

        private bool IsCorrectNpcOpened(Player player)
        {
            if (player.OpenedNpc == null || player.OpenedNpc.Definition.NpcWindow != NpcWindow.Lahap)
            {
                player.Logger.LogWarning("Probably Hacker tried to Mix/Unmix Jewels without talking to Lahap. Dupe Method. Acc: [{0}] Character: [{1}]", player.Account.LoginName, player.SelectedCharacter.Name);
                player.Disconnect();
                return false;
            }

            return true;
        }

        private JewelMix GetJewelMix(byte mixId, Player player)
        {
            JewelMix mix = player.GameContext.Configuration.JewelMixes.FirstOrDefault(m => m.Number == mixId);
            if (mix == null)
            {
                player.Logger.LogWarning($"Unkown mix type [{mixId}], Player Name: [{player.SelectedCharacter?.Name}], Account Name: [{player.Account?.LoginName}]");
            }

            return mix;
        }
    }
}
