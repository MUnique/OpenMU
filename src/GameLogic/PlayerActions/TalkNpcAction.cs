// <copyright file="TalkNpcAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions;

using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions.Quests;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;
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
                    else if (player.CurrentMiniGame is IllusionTempleContext illusionTemple)
                    {
                        switch (player.OpenedNpc.Definition.Number)
                        {
                            case 380: // Stone Statue
                                await illusionTemple.TalkToNpcStoneStatueAsync(player).ConfigureAwait(false);
                                break;
                            case 383: // Alliance Item Storage
                            case 384: // Illusion Item Storage
                                await illusionTemple.TalkToNpcTeamStorageAsync(player.OpenedNpc.Definition.Number, player).ConfigureAwait(false);
                                break;
                            default:
                                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.TalkingNotImplementedFormat), npcStats.Number, npcStats.Designation).ConfigureAwait(false);
                                break;
                        }
                    }
                    else
                    {
                        await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.TalkingNotImplementedFormat), npcStats.Number, npcStats.Designation).ConfigureAwait(false);
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
            case NpcWindow.ChaosMachine:
            case NpcWindow.RemoveJohOption:
                await player.InvokeViewPlugInAsync<IOpenNpcWindowPlugIn>(p => p.OpenNpcWindowAsync(npcStats.NpcWindow)).ConfigureAwait(false);
                break;
            case NpcWindow.IllusionTemple:
                try
                {
                    await player.InvokeViewPlugInAsync<IOpenNpcWindowPlugIn>(p => p.OpenNpcWindowAsync(npcStats.NpcWindow)).ConfigureAwait(false);
                    await this.ShowIllusionTempleUserCountsAsync(player).ConfigureAwait(false);
                }
                finally
                {
                    // The client doesn't tell the server when this window is closed, so the state is reset
                    // right away - otherwise the player would be stuck in the NpcDialogOpened state and
                    // couldn't open the window a second time. That has to happen even when showing the
                    // dialog failed, for the same reason. The npc itself stays assigned, so that the
                    // entry can still report its refusals as a message of the npc.
                    await player.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
                }

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

    /// <summary>
    /// Sends the number of players of each illusion temple to the client, so that it can show them
    /// in the entrance dialog next to the temples the player can enter.
    /// </summary>
    /// <param name="player">The player which opened the illusion temple dialog.</param>
    private async ValueTask ShowIllusionTempleUserCountsAsync(Player player)
    {
        if (player.GameContext.PlugInManager.GetStrategy<MiniGameType, IPeriodicMiniGameStartPlugIn>(MiniGameType.IllusionTemple) is not { } startPlugIn)
        {
            // The event is not enabled on this server - the dialog then just shows no members at all.
            return;
        }

        var definitions = player.GameContext.Configuration.MiniGameDefinitions
            .Where(definition => definition.Type == MiniGameType.IllusionTemple)
            .OrderBy(definition => definition.GameLevel)
            .ToList();

        var userCounts = new List<int>(definitions.Count);
        foreach (var definition in definitions)
        {
            // GetMiniGameContextAsync returns null when the event isn't running - in contrast to
            // IGameContext.GetMiniGameAsync, which would create a context and thereby start all
            // six temples just by asking for their player count.
            var miniGameContext = await startPlugIn.GetMiniGameContextAsync(player.GameContext, definition).ConfigureAwait(false);
            userCounts.Add(miniGameContext?.PlayerCount ?? 0);
        }

        await player.InvokeViewPlugInAsync<IShowIllusionTempleUserCountViewPlugIn>(p => p.ShowUserCountAsync(userCounts)).ConfigureAwait(false);
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