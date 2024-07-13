// <copyright file="TalkNpcAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions;

using MUnique.OpenMU.GameLogic.MiniGames;
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

        if (this.AdvancePlayerState(npc))
        {
            if (!await player.PlayerState.TryAdvanceToAsync(PlayerState.NpcDialogOpened).ConfigureAwait(false))
            {
                return;
            }
        }
        else
        {
            if (player.PlayerState.CurrentState != PlayerState.EnteredWorld)
            {
                return;
            }
        }

        player.OpenedNpc = npc;
        if (npcStats.MerchantStore != null && npcStats.MerchantStore.Items.Count > 0)
        {
            await Task.Delay(500).ConfigureAwait(false);
            await player.InvokeViewPlugInAsync<IOpenNpcWindowPlugIn>(p => p.OpenNpcWindowAsync(npcStats.NpcWindow != NpcWindow.Undefined ? npcStats.NpcWindow : NpcWindow.Merchant)).ConfigureAwait(false);
            await player.InvokeViewPlugInAsync<IShowMerchantStoreItemListPlugIn>(p => p.ShowMerchantStoreItemListAsync(npcStats.MerchantStore.Items, StoreKind.Normal)).ConfigureAwait(false);
        }
        else
        {
            await this.ShowDialogOfOpenedNpcAsync(player).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Gets a value indicating whether this action advances the player state to <see cref="PlayerState.NpcDialogOpened" />.
    /// </summary>
    /// <param name="npc">The NPC whose dialog is about to be opened.</param>
    /// <returns>A value indicating whether this action advances the player state to <see cref="PlayerState.NpcDialogOpened" />.</returns>
    protected virtual bool AdvancePlayerState(NonPlayerCharacter npc) => true;

    private async ValueTask ShowDialogOfOpenedNpcAsync(Player player)
    {
        var npcStats = player.OpenedNpc!.Definition;
        switch (npcStats.NpcWindow)
        {
            case NpcWindow.Undefined:
                var eventArgs = new NpcTalkEventArgs();
                player.GameContext.PlugInManager.GetPlugInPoint<IPlayerTalkToNpcPlugIn>()?.PlayerTalksToNpcAsync(player, player.OpenedNpc, eventArgs);
                if (!eventArgs.HasBeenHandled)
                {
                    if (player.CurrentMiniGame is BloodCastleContext bloodCastle && player.OpenedNpc.Definition.Number == 232)
                    {
                        await bloodCastle.TalkToNpcArchangelAsync(player).ConfigureAwait(false);
                    }
                    else
                    {
                        await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync($"Talking to this NPC ({npcStats.Number}, {npcStats.Designation}) is not implemented yet.", MessageType.BlueNormal)).ConfigureAwait(false);
                    }

                    await player.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
                }
                else if (!eventArgs.LeavesDialogOpen)
                {
                    player.OpenedNpc = null;
                    await player.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
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
                if (await this.IsPlayedAllowedToCreateGuildAsync(player).ConfigureAwait(false))
                {
                    await player.InvokeViewPlugInAsync<IShowGuildMasterDialogPlugIn>(p => p.ShowGuildMasterDialogAsync()).ConfigureAwait(false);
                }
                else
                {
                    player.OpenedNpc = null;
                    await player.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
                }

                break;
            case NpcWindow.LegacyQuest:
                await this.ShowLegacyQuestDialogAsync(player).ConfigureAwait(false);
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
            default:
                await player.InvokeViewPlugInAsync<IOpenNpcWindowPlugIn>(p => p.OpenNpcWindowAsync(npcStats.NpcWindow)).ConfigureAwait(false);
                break;
        }
    }

    private async ValueTask ShowLegacyQuestDialogAsync(Player player)
    {
        var quests = player.OpenedNpc!.Definition.Quests
            .Where(q => q.QualifiedCharacter is null || Equals(q.QualifiedCharacter, player.SelectedCharacter!.CharacterClass));

        if (!quests.Any())
        {
            await player.InvokeViewPlugInAsync<IShowMessageOfObjectPlugIn>(p => p.ShowMessageOfObjectAsync("I have no quests for you.", player.OpenedNpc)).ConfigureAwait(false);
            player.OpenedNpc = null;
            await player.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
            return;
        }

        if (quests.All(quest => quest.MinimumCharacterLevel > player.Level))
        {
            await player.InvokeViewPlugInAsync<IShowMessageOfObjectPlugIn>(p => p.ShowMessageOfObjectAsync(
                "I have nothing to do for you. Come back with more power.",
                player.OpenedNpc)).ConfigureAwait(false);
            player.OpenedNpc = null;
            await player.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
            return;
        }

        var maxQuestNumber = quests.Max(q => q.Number);
        var questGroup = quests.FirstOrDefault(q => q.Number == maxQuestNumber)?.Group;
        var questState = player.GetQuestState(questGroup ?? 0);
        if (questState?.LastFinishedQuest?.Number >= maxQuestNumber)
        {
            await player.InvokeViewPlugInAsync<IShowMessageOfObjectPlugIn>(p => p.ShowMessageOfObjectAsync(
                "I have nothing to do for you. You solved all my quests already.",
                player.OpenedNpc)).ConfigureAwait(false);
            player.OpenedNpc = null;
            await player.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
            return;
        }

        await player.InvokeViewPlugInAsync<ILegacyQuestStateDialogPlugIn>(p => p.ShowAsync()).ConfigureAwait(false);
    }

    private async ValueTask<bool> IsPlayedAllowedToCreateGuildAsync(Player player)
    {
        if (player.Level < 100)
        {
            await player.InvokeViewPlugInAsync<IShowMessageOfObjectPlugIn>(p => p.ShowMessageOfObjectAsync("Your level should be at least level 100", player.OpenedNpc!)).ConfigureAwait(false);
            return false;
        }

        if (player.GuildStatus != null)
        {
            await player.InvokeViewPlugInAsync<IShowMessageOfObjectPlugIn>(p => p.ShowMessageOfObjectAsync("You already belong to a guild", player.OpenedNpc!)).ConfigureAwait(false);
            return false;
        }

        return true;
    }
}