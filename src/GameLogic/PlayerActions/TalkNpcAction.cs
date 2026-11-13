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
using MUnique.OpenMU.GameLogic.Views.NPC;
using MUnique.OpenMU.GameLogic.Views.Quest;
using MUnique.OpenMU.GameLogic.Views.Vault;
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

        var advancePlayerState = this.AdvancePlayerState(npc);
        var expectedState = advancePlayerState ? PlayerState.NpcDialogOpened : PlayerState.EnteredWorld;
        var dialog = new Player.NpcDialogContext(npc);
        await using (var context = await player.PlayerState.TryBeginAdvanceToAsync(expectedState).ConfigureAwait(false))
        {
            if (advancePlayerState)
            {
                if (!context.Allowed)
                {
                    return;
                }
            }
            else
            {
                // Older clients keep ordinary NPC responses in EnteredWorld.
                context.Allowed = false;
                if (player.PlayerState.CurrentState != expectedState)
                {
                    return;
                }
            }

            player.OpenedNpcContext = dialog;
        }

        if (npcStats.MerchantStore != null && npcStats.MerchantStore.Items.Count > 0)
        {
            await Task.Delay(500).ConfigureAwait(false);
            await player.InvokeViewPlugInAsync<IOpenNpcWindowPlugIn>(p => p.OpenNpcWindowAsync(npcStats.NpcWindow != NpcWindow.Undefined ? npcStats.NpcWindow : NpcWindow.Merchant)).ConfigureAwait(false);
            await player.InvokeViewPlugInAsync<IShowMerchantStoreItemListPlugIn>(p => p.ShowMerchantStoreItemListAsync(npcStats.MerchantStore.GetOfferedItems(), StoreKind.Normal)).ConfigureAwait(false);
        }
        else
        {
            await this.ShowDialogOfOpenedNpcAsync(player, dialog, expectedState).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Gets a value indicating whether this action advances the player state to <see cref="PlayerState.NpcDialogOpened" />.
    /// </summary>
    /// <param name="npc">The NPC whose dialog is about to be opened.</param>
    /// <returns>A value indicating whether this action advances the player state to <see cref="PlayerState.NpcDialogOpened" />.</returns>
    protected virtual bool AdvancePlayerState(NonPlayerCharacter npc) => true;

    private static async ValueTask<bool> CompleteDialogAsync(Player player, Player.NpcDialogContext dialog, State expectedState, bool clearNpc = true)
    {
        await using var context = await player.PlayerState.TryBeginAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        var ownsDialog = player.PlayerState.CurrentState == expectedState
            && ReferenceEquals(player.OpenedNpcContext, dialog);
        context.Allowed &= ownsDialog;
        if (ownsDialog && clearNpc)
        {
            player.OpenedNpc = null;
        }

        return ownsDialog;
    }

    private async ValueTask ShowDialogOfOpenedNpcAsync(Player player, Player.NpcDialogContext dialog, State expectedState)
    {
        var npcStats = dialog.Npc.Definition;
        switch (npcStats.NpcWindow)
        {
            case NpcWindow.Undefined:
                var eventArgs = new NpcTalkEventArgs();
                if (player.GameContext.PlugInManager.GetPlugInPoint<IPlayerTalkToNpcPlugIn>() is { } talkToNpcPlugIn)
                {
                    await talkToNpcPlugIn.PlayerTalksToNpcAsync(player, dialog.Npc, eventArgs).ConfigureAwait(false);
                }

                if (!eventArgs.HasBeenHandled)
                {
                    await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.TalkingNotImplementedFormat), npcStats.Number, npcStats.Designation).ConfigureAwait(false);

                    if (!await CompleteDialogAsync(player, dialog, expectedState, clearNpc: false).ConfigureAwait(false))
                    {
                        return;
                    }
                }
                else if (!eventArgs.LeavesDialogOpen)
                {
                    if (!await CompleteDialogAsync(player, dialog, expectedState).ConfigureAwait(false))
                    {
                        return;
                    }
                }
                else
                {
                    // Leaves dialog opened, so leave the state as it is.
                }

                break;
            case NpcWindow.VaultStorage:
                player.Account!.Vault ??= player.PersistenceContext.CreateNew<ItemStorage>();
                var warehouseSize = player.Account.IsVaultExtended ? InventoryConstants.WarehouseSize * 2 : InventoryConstants.WarehouseSize;
                player.Vault = new Storage(warehouseSize, player.Account.Vault);
                await player.InvokeViewPlugInAsync<IShowVaultPlugIn>(p => p.ShowVaultAsync()).ConfigureAwait(false);
                break;
            case NpcWindow.GuildMaster:
                if (await this.IsPlayedAllowedToCreateGuildAsync(player, dialog.Npc).ConfigureAwait(false))
                {
                    await player.InvokeViewPlugInAsync<IShowGuildMasterDialogPlugIn>(p => p.ShowGuildMasterDialogAsync()).ConfigureAwait(false);
                }
                else
                {
                    if (!await CompleteDialogAsync(player, dialog, expectedState).ConfigureAwait(false))
                    {
                        return;
                    }
                }

                break;
            case NpcWindow.LegacyQuest:
                if (!await this.ShowLegacyQuestDialogAsync(player, dialog, expectedState).ConfigureAwait(false))
                {
                    return;
                }

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

        if (npcStats.ItemCraftings.Any())
        {
            player.BackupInventory = new BackupItemStorage(player.Inventory!.ItemStorage);
        }
    }

    private async ValueTask<bool> ShowLegacyQuestDialogAsync(Player player, Player.NpcDialogContext dialog, State expectedState)
    {
        var quests = dialog.Npc.Definition.Quests
            .Where(q => q.QualifiedCharacter is null || Equals(q.QualifiedCharacter, player.SelectedCharacter!.CharacterClass));

        if (!quests.Any())
        {
            await player.InvokeViewPlugInAsync<IShowMessageOfObjectPlugIn>(p => p.ShowMessageOfObjectAsync("I have no quests for you.", dialog.Npc)).ConfigureAwait(false);
            return await CompleteDialogAsync(player, dialog, expectedState).ConfigureAwait(false);
        }

        if (quests.All(quest => quest.MinimumCharacterLevel > player.Level))
        {
            await player.InvokeViewPlugInAsync<IShowMessageOfObjectPlugIn>(p => p.ShowMessageOfObjectAsync(
                "I have nothing to do for you. Come back with more power.",
                dialog.Npc)).ConfigureAwait(false);
            return await CompleteDialogAsync(player, dialog, expectedState).ConfigureAwait(false);
        }

        var maxQuestNumber = quests.Max(q => q.Number);
        var questGroup = quests.FirstOrDefault(q => q.Number == maxQuestNumber)?.Group;
        var questState = player.GetQuestState(questGroup ?? 0);
        if (questState?.LastFinishedQuest?.Number >= maxQuestNumber)
        {
            await player.InvokeViewPlugInAsync<IShowMessageOfObjectPlugIn>(p => p.ShowMessageOfObjectAsync(
                "I have nothing to do for you. You solved all my quests already.",
                dialog.Npc)).ConfigureAwait(false);
            return await CompleteDialogAsync(player, dialog, expectedState).ConfigureAwait(false);
        }

        await player.InvokeViewPlugInAsync<ILegacyQuestStateDialogPlugIn>(p => p.ShowAsync()).ConfigureAwait(false);
        return true;
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