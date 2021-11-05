// <copyright file="EnterMiniGameAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.MiniGames
{
    using System;
    using System.IO;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.MiniGames;
    using MUnique.OpenMU.GameLogic.Views.Inventory;

    /// <summary>
    /// Player action which implements entering a mini game.
    /// </summary>
    public class EnterMiniGameAction
    {
        /// <summary>
        /// Tries to enter the mini game event of the specified type and level with the specified ticket item.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="miniGameType">The mini game type.</param>
        /// <param name="gameLevel">The mini game level.</param>
        /// <param name="gameTicketInventoryIndex">The inventory index of the ticket.</param>
        public void TryEnterMiniGame(Player player, MiniGameType miniGameType, int gameLevel, byte gameTicketInventoryIndex)
        {
            if (player.SelectedCharacter?.CharacterClass is null)
            {
                throw new InvalidOperationException("Character not selected or initialized.");
            }

            var miniGameDefinition = player.GameContext.Configuration.MiniGameDefinitions
                .FirstOrDefault(def => def.Type == miniGameType && def.GameLevel == gameLevel);
            if (miniGameDefinition is null || (miniGameDefinition.RequiresMasterClass && !player.SelectedCharacter.CharacterClass.IsMasterClass))
            {
                player.ViewPlugIns.GetPlugIn<IShowMiniGameEnterResultPlugIn>()?.ShowResult(miniGameType, EnterResult.Failed);
                return;
            }

            // todo: consider special characters
            var characterLevel = player.Attributes![Stats.Level];
            if (characterLevel < miniGameDefinition.MinimumCharacterLevel)
            {
                player.ViewPlugIns.GetPlugIn<IShowMiniGameEnterResultPlugIn>()?.ShowResult(miniGameType, EnterResult.CharacterLevelTooLow);
                return;
            }

            if (characterLevel > miniGameDefinition.MaximumCharacterLevel)
            {
                player.ViewPlugIns.GetPlugIn<IShowMiniGameEnterResultPlugIn>()?.ShowResult(miniGameType, EnterResult.CharacterLevelTooHigh);
                return;
            }

            if (!this.CheckTicketItem(miniGameDefinition, player, gameTicketInventoryIndex, out var ticketItem))
            {
                player.ViewPlugIns.GetPlugIn<IShowMiniGameEnterResultPlugIn>()?.ShowResult(miniGameType, EnterResult.Failed);
                return;
            }

            var entrance = miniGameDefinition.Entrance ?? throw new InvalidDataException("mini game entrance not defined");
            var miniGame = player.GameContext.GetMiniGame(miniGameDefinition, player);
            if (miniGame.TryEnter(player, out var result))
            {
                this.ConsumeTicketItem(ticketItem, player);
                player.WarpTo(entrance);
            }
            else
            {
                player.ViewPlugIns.GetPlugIn<IShowMiniGameEnterResultPlugIn>()?.ShowResult(miniGameType, result);
            }
        }

        private void ConsumeTicketItem(Item? ticketItem, Player player)
        {
            if (ticketItem is null)
            {
                return;
            }

            if (ticketItem.Durability > 0)
            {
                ticketItem.Durability -= 1;
            }

            if (ticketItem.Durability == 0)
            {
                var slot = ticketItem.ItemSlot;
                player.Inventory?.RemoveItem(ticketItem);
                player.PersistenceContext.Delete(ticketItem);
                player.ViewPlugIns.GetPlugIn<Views.Inventory.IItemRemovedPlugIn>()?.RemoveItem(slot);
            }
            else
            {
                player.ViewPlugIns.GetPlugIn<IItemDurabilityChangedPlugIn>()?.ItemDurabilityChanged(ticketItem, false);
            }
        }

        private bool CheckTicketItem(MiniGameDefinition miniGameDefinition, Player player, byte gameTicketInventoryIndex, out Item? ticketItem)
        {
            if (miniGameDefinition.TicketItem is not { } ticketItemDefinition)
            {
                ticketItem = null;
                return true;
            }

            ticketItem = player.Inventory?.GetItem(gameTicketInventoryIndex);
            return ticketItemDefinition == ticketItem?.Definition && ticketItem.Durability > 0 && ticketItem.Level == miniGameDefinition.TicketItemLevel;
        }
    }
}
