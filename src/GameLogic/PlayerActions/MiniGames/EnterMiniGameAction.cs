// <copyright file="EnterMiniGameAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.MiniGames;

using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.GuildWar;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.PlayerActions.PlayerStore;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;
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
    public async ValueTask TryEnterMiniGameAsync(Player player, MiniGameType miniGameType, int gameLevel, byte gameTicketInventoryIndex)
    {
        if (player.SelectedCharacter?.CharacterClass is null)
        {
            throw new InvalidOperationException("Character not selected or initialized.");
        }

        var miniGameDefinition = player.GameContext.Configuration.MiniGameDefinitions
            .FirstOrDefault(def => def.Type == miniGameType && def.GameLevel == gameLevel);
        if (miniGameDefinition is null
            || (miniGameDefinition.RequiresMasterClass && !player.SelectedCharacter.CharacterClass.IsMasterClass)
            || player.CurrentMiniGame is not null)
        {
            await player.InvokeViewPlugInAsync<IShowMiniGameEnterResultPlugIn>(p => p.ShowResultAsync(miniGameType, EnterResult.Failed)).ConfigureAwait(false);
            return;
        }

        var isSpecialCharacter = player.SelectedCharacter.IsSpecialCharacter();
        var characterLevel = player.Attributes![Stats.Level];
        var minLevel = isSpecialCharacter ? miniGameDefinition.MinimumSpecialCharacterLevel : miniGameDefinition.MinimumCharacterLevel;
        var maxLevel = isSpecialCharacter ? miniGameDefinition.MaximumSpecialCharacterLevel : miniGameDefinition.MaximumCharacterLevel;
        var requiresMasterLevel = miniGameDefinition.RequiresMasterClass;
        if (characterLevel < minLevel || (requiresMasterLevel && player.SelectedCharacter?.CharacterClass?.IsMasterClass is not true))
        {
            await player.InvokeViewPlugInAsync<IShowMiniGameEnterResultPlugIn>(p => p.ShowResultAsync(miniGameType, EnterResult.CharacterLevelTooLow)).ConfigureAwait(false);
            return;
        }

        if (characterLevel > maxLevel)
        {
            await player.InvokeViewPlugInAsync<IShowMiniGameEnterResultPlugIn>(p => p.ShowResultAsync(miniGameType, EnterResult.CharacterLevelTooHigh)).ConfigureAwait(false);
            return;
        }

        if (!this.CheckTicketItem(miniGameDefinition, player, gameTicketInventoryIndex, out var ticketItem))
        {
            await player.InvokeViewPlugInAsync<IShowMiniGameEnterResultPlugIn>(p => p.ShowResultAsync(miniGameType, EnterResult.Failed)).ConfigureAwait(false);
            return;
        }

        if (!this.CheckEntranceFee(miniGameDefinition, player, out var entranceFee))
        {
            await player.InvokeViewPlugInAsync<IShowMiniGameEnterResultPlugIn>(p => p.ShowResultAsync(miniGameType, EnterResult.NotEnoughMoney)).ConfigureAwait(false);
            return;
        }

        if (!this.CheckPlayerKillState(miniGameDefinition, player))
        {
            await player.InvokeViewPlugInAsync<IShowMiniGameEnterResultPlugIn>(p => p.ShowResultAsync(miniGameType, EnterResult.PlayerKillerCantEnter)).ConfigureAwait(false);
            return;
        }

        if (player.GuildWarContext is { State: GuildWarState.Started or GuildWarState.Requested })
        {
            await player.InvokeViewPlugInAsync<IShowMiniGameEnterResultPlugIn>(p => p.ShowResultAsync(miniGameType, EnterResult.Failed)).ConfigureAwait(false);
            return;
        }

        // TODO: Check Duel state when duels are implemented

        if (miniGameDefinition.MapCreationPolicy == MiniGameMapCreationPolicy.Shared)
        {
            var miniGameStrategy = player.GameContext.PlugInManager.GetStrategy<MiniGameType, IPeriodicMiniGameStartPlugIn>(miniGameDefinition.Type);
            if (miniGameStrategy is not null
                && await miniGameStrategy.GetDurationUntilNextStartAsync(player.GameContext, miniGameDefinition).ConfigureAwait(false) != TimeSpan.Zero)
            {
                await player.InvokeViewPlugInAsync<IShowMiniGameEnterResultPlugIn>(p => p.ShowResultAsync(miniGameType, EnterResult.NotOpen)).ConfigureAwait(false);
                return;
            }
        }

        var entrance = miniGameDefinition.Entrance ?? throw new InvalidOperationException("mini game entrance not defined");
        var miniGame = await player.GameContext.GetMiniGameAsync(miniGameDefinition, player).ConfigureAwait(false);

        var enterResult = await miniGame.TryEnterAsync(player).ConfigureAwait(false);
        if (enterResult == EnterResult.Success)
        {
            if (entranceFee > 0)
            {
                player.TryRemoveMoney(entranceFee);
            }

            await this.ConsumeTicketItemAsync(ticketItem, player).ConfigureAwait(false);

            if (!miniGameDefinition.AllowParty && player.Party is { } party)
            {
                await party.KickMySelfAsync(player).ConfigureAwait(false);
            }

            await player.MagicEffectList.ClearEffectsAfterDeathAsync().ConfigureAwait(false);

            if (player.ShopStorage?.StoreOpen ?? false)
            {
                var storeCloseAction = new CloseStoreAction();
                await storeCloseAction.CloseStoreAsync(player).ConfigureAwait(false);
            }

            await player.RemoveSummonAsync().ConfigureAwait(false);
            await player.MagicEffectList.ClearEffectsAfterDeathAsync().ConfigureAwait(false);
            await player.WarpToAsync(entrance).ConfigureAwait(false);
        }
        else
        {
            await player.InvokeViewPlugInAsync<IShowMiniGameEnterResultPlugIn>(p => p.ShowResultAsync(miniGameType, enterResult)).ConfigureAwait(false);
        }
    }

    private bool CheckPlayerKillState(MiniGameDefinition miniGameDefinition, Player player)
    {
        if (miniGameDefinition.ArePlayerKillersAllowedToEnter)
        {
            return true;
        }

        return player.SelectedCharacter?.State < HeroState.PlayerKiller1stStage;
    }

    private async ValueTask ConsumeTicketItemAsync(Item? ticketItem, Player player)
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
            if (player.Inventory is { } inventory)
            {
                await inventory.RemoveItemAsync(ticketItem).ConfigureAwait(false);
            }

            await player.DestroyInventoryItemAsync(ticketItem).ConfigureAwait(false);
        }
        else
        {
            await player.InvokeViewPlugInAsync<IItemDurabilityChangedPlugIn>(p => p.ItemDurabilityChangedAsync(ticketItem, false)).ConfigureAwait(false);
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
        return ticketItemDefinition.Equals(ticketItem?.Definition) && ticketItem.Durability > 0 && ticketItem.Level == miniGameDefinition.TicketItemLevel;
    }

    private bool CheckEntranceFee(MiniGameDefinition miniGameDefinition, Player player, out int entranceFee)
    {
        entranceFee = Math.Max(miniGameDefinition.EntranceFee, 0);
        return entranceFee <= player.Money;
    }
}