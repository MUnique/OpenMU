// <copyright file="TalkNpcAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions;

using MUnique.OpenMU.GameLogic.Crywolf;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions.Quests;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.GameLogic.Views.NPC;
using MUnique.OpenMU.GameLogic.Views.Quest;
using MUnique.OpenMU.GameLogic.Views.Vault;
using MUnique.OpenMU.GameLogic.WaitingItems;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Action to talk to a npc.
/// </summary>
public class TalkNpcAction
{
    /// <summary>
    /// Talks to the specified Monster.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="npc">The Monster.</param>
    public async ValueTask TalkToNpcAsync(Player player, NonPlayerCharacter npc)
    {
        var npcStats = npc.Definition;
        if (CrywolfPlugIn.GetContext(player.GameContext) is { } crywolf && crywolf.IsNpcHidden(npc))
        {
            // The NPCs of the crywolf fortress are hidden while it isn't in peace, so they can't be used.
            return;
        }

        var isMerchant = npcStats.MerchantStore != null && npcStats.MerchantStore.Items.Count > 0;
        var advancePlayerState = this.AdvancePlayerState(npc);
        var returnedLeftovers = false;
        await using (var context = await player.PlayerState.TryBeginAdvanceToAsync(advancePlayerState ? PlayerState.NpcDialogOpened : PlayerState.EnteredWorld).ConfigureAwait(false))
        {
            if (!advancePlayerState)
            {
                // Older clients keep ordinary NPC responses in the entered world state.
                context.Allowed = player.PlayerState.CurrentState == PlayerState.EnteredWorld;
                if (!context.Allowed)
                {
                    return;
                }

                context.Allowed = false;
            }

            if (advancePlayerState && !context.Allowed)
            {
                return;
            }

            // A life which is dying or relocated by force can't open a dialog: death and forced
            // placement close the open interactions, and must not get a new one each time.
            if (!player.IsAlive || player.IsPreparingForcedPlacement)
            {
                context.Allowed = false;
                return;
            }

            player.OpenedNpc = npc;
            if (!isMerchant && npcStats.ItemCraftings.Any())
            {
                // Leftovers of a crafting dialog which was left without closing it properly.
                returnedLeftovers = await player.TryReturnTemporaryStorageItemsAsync().ConfigureAwait(false);

                // Taken together with the opening: a response which continues after this dialog was closed
                // (e.g. by a death) must not publish a backup which a later disconnect would restore.
                player.BackupInventory = new BackupItemStorage(player.Inventory!.ItemStorage);
            }
        }

        if (returnedLeftovers)
        {
            await player.InvokeViewPlugInAsync<IUpdateInventoryListPlugIn>(p => p.UpdateInventoryListAsync()).ConfigureAwait(false);
        }

        if (isMerchant)
        {
            await Task.Delay(500).ConfigureAwait(false);
            await player.InvokeViewPlugInAsync<IOpenNpcWindowPlugIn>(p => p.OpenNpcWindowAsync(npcStats.NpcWindow != NpcWindow.Undefined ? npcStats.NpcWindow : NpcWindow.Merchant)).ConfigureAwait(false);
            await player.InvokeViewPlugInAsync<IShowMerchantStoreItemListPlugIn>(p => p.ShowMerchantStoreItemListAsync(npcStats.MerchantStore!.GetOfferedItems(), StoreKind.Normal)).ConfigureAwait(false);
        }
        else
        {
            await this.ShowDialogOfOpenedNpcAsync(player, npc).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Gets a value indicating whether this action advances the player state to <see cref="PlayerState.NpcDialogOpened" />.
    /// </summary>
    /// <param name="npc">The NPC whose dialog is about to be opened.</param>
    /// <returns>A value indicating whether this action advances the player state to <see cref="PlayerState.NpcDialogOpened" />.</returns>
    protected virtual bool AdvancePlayerState(NonPlayerCharacter npc) => true;

    /// <summary>
    /// Ends the dialog with the NPC when it's still the opened one. A late response of a dialog which a death,
    /// a map change or a close already ended doesn't change the state of the player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="npc">The NPC of the responding dialog.</param>
    /// <param name="clearNpc">Whether the opened NPC is cleared, too.</param>
    private static async ValueTask CompleteDialogAsync(Player player, NonPlayerCharacter npc, bool clearNpc = true)
    {
        await using var context = await player.PlayerState.TryBeginAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        var state = player.PlayerState.CurrentState;
        var isOpened = ReferenceEquals(player.OpenedNpc, npc)
            && (state == PlayerState.NpcDialogOpened || state == PlayerState.EnteredWorld);
        context.Allowed &= isOpened && state == PlayerState.NpcDialogOpened;
        if (isOpened && clearNpc)
        {
            player.OpenedNpc = null;
        }
    }

    private async ValueTask ShowDialogOfOpenedNpcAsync(Player player, NonPlayerCharacter npc)
    {
        var npcStats = npc.Definition;
        switch (npcStats.NpcWindow)
        {
            case NpcWindow.Undefined:
                var eventArgs = new NpcTalkEventArgs();
                if (player.GameContext.PlugInManager.GetPlugInPoint<IPlayerTalkToNpcPlugIn>() is { } talkToNpcPlugIn)
                {
                    await talkToNpcPlugIn.PlayerTalksToNpcAsync(player, npc, eventArgs).ConfigureAwait(false);
                }

                if (!eventArgs.HasBeenHandled)
                {
                    await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.TalkingNotImplementedFormat), npcStats.Number, npcStats.Designation).ConfigureAwait(false);

                    await CompleteDialogAsync(player, npc, clearNpc: false).ConfigureAwait(false);
                }
                else if (!eventArgs.LeavesDialogOpen)
                {
                    await CompleteDialogAsync(player, npc).ConfigureAwait(false);
                }
                else
                {
                    // Leaves dialog opened, so leave the state as it is.
                }

                break;
            case NpcWindow.VaultStorage:
                player.Account!.Vault ??= player.PersistenceContext.CreateNew<ItemStorage>();
                var warehouseSize = player.Account.IsVaultExtended ? InventoryConstants.WarehouseSize * 2 : InventoryConstants.WarehouseSize;
                var character = player.SelectedCharacter!;
                var waitingItemCount = character.WaitingItems?.Count ?? 0;
                player.Vault = new Storage(warehouseSize, 0, 0, player.Account.Vault, player.PutAsideWhenLoaded(character));
                await player.InvokeViewPlugInAsync<IShowVaultPlugIn>(p => p.ShowVaultAsync()).ConfigureAwait(false);
                if ((character.WaitingItems?.Count ?? 0) > waitingItemCount
                    && WaitingItemsDeliveryPlugIn.GetActive(player.GameContext) is { } delivery)
                {
                    await delivery.DeliverAndShowAsync(player).ConfigureAwait(false);
                }

                break;
            case NpcWindow.GuildMaster:
                if (await this.IsPlayedAllowedToCreateGuildAsync(player, npc).ConfigureAwait(false))
                {
                    await player.InvokeViewPlugInAsync<IShowGuildMasterDialogPlugIn>(p => p.ShowGuildMasterDialogAsync()).ConfigureAwait(false);
                }
                else
                {
                    await CompleteDialogAsync(player, npc).ConfigureAwait(false);
                }

                break;
            case NpcWindow.LegacyQuest:
                await this.ShowLegacyQuestDialogAsync(player, npc).ConfigureAwait(false);
                break;
            case NpcWindow.DoorkeeperTitusDuelWatch:
                await player.InvokeViewPlugInAsync<IOpenNpcWindowPlugIn>(p => p.OpenNpcWindowAsync(npcStats.NpcWindow)).ConfigureAwait(false);
                _ = Task.Run(async () =>
                {
                    while (player.IsActive() && player.OpenedNpc?.Definition.NpcWindow == NpcWindow.DoorkeeperTitusDuelWatch)
                    {
                        await player.GameContext.DuelRoomManager.ShowRoomsAsync(player).ConfigureAwait(false);
                        await Task.Delay(5000).ConfigureAwait(false);
                    }
                });

                break;
            case NpcWindow.ChaosMachine:
            case NpcWindow.RemoveJohOption:
                await player.InvokeViewPlugInAsync<IOpenNpcWindowPlugIn>(p => p.OpenNpcWindowAsync(npcStats.NpcWindow)).ConfigureAwait(false);
                break;
            default:
                await player.InvokeViewPlugInAsync<IOpenNpcWindowPlugIn>(p => p.OpenNpcWindowAsync(npcStats.NpcWindow)).ConfigureAwait(false);
                break;
        }
    }

    private async ValueTask ShowLegacyQuestDialogAsync(Player player, NonPlayerCharacter npc)
    {
        var quests = npc.Definition.Quests
            .Where(q => q.QualifiedCharacter is null || Equals(q.QualifiedCharacter, player.SelectedCharacter!.CharacterClass));

        if (!quests.Any())
        {
            await player.InvokeViewPlugInAsync<IShowMessageOfObjectPlugIn>(p => p.ShowMessageOfObjectAsync("I have no quests for you.", npc)).ConfigureAwait(false);
            await CompleteDialogAsync(player, npc).ConfigureAwait(false);
            return;
        }

        if (quests.All(quest => quest.MinimumCharacterLevel > player.Level))
        {
            await player.InvokeViewPlugInAsync<IShowMessageOfObjectPlugIn>(p => p.ShowMessageOfObjectAsync(
                "I have nothing to do for you. Come back with more power.",
                npc)).ConfigureAwait(false);
            await CompleteDialogAsync(player, npc).ConfigureAwait(false);
            return;
        }

        var maxQuestNumber = quests.Max(q => q.Number);
        var questGroup = quests.FirstOrDefault(q => q.Number == maxQuestNumber)?.Group;
        var questState = player.GetQuestState(questGroup ?? 0);
        if (questState?.LastFinishedQuest?.Number >= maxQuestNumber)
        {
            await player.InvokeViewPlugInAsync<IShowMessageOfObjectPlugIn>(p => p.ShowMessageOfObjectAsync(
                "I have nothing to do for you. You solved all my quests already.",
                npc)).ConfigureAwait(false);
            await CompleteDialogAsync(player, npc).ConfigureAwait(false);
            return;
        }

        await player.InvokeViewPlugInAsync<ILegacyQuestStateDialogPlugIn>(p => p.ShowAsync()).ConfigureAwait(false);
    }

    private async ValueTask<bool> IsPlayedAllowedToCreateGuildAsync(Player player, NonPlayerCharacter npc)
    {
        if (player.Level < 100)
        {
            await player.InvokeViewPlugInAsync<IShowMessageOfObjectPlugIn>(p => p.ShowMessageOfObjectAsync("Your level should be at least level 100", npc)).ConfigureAwait(false);
            return false;
        }

        if (player.GuildStatus != null)
        {
            await player.InvokeViewPlugInAsync<IShowMessageOfObjectPlugIn>(p => p.ShowMessageOfObjectAsync("You already belong to a guild", npc)).ConfigureAwait(false);
            return false;
        }

        return true;
    }
}