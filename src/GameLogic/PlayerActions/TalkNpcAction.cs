// <copyright file="TalkNpcAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions
{
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
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
        public void TalkToNpc(Player player, NonPlayerCharacter npc)
        {
            var npcStats = npc.Definition;

            if (this.AdvancePlayerState(npc))
            {
                if (!player.PlayerState.TryAdvanceTo(PlayerState.NpcDialogOpened))
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
                player.ViewPlugIns.GetPlugIn<IOpenNpcWindowPlugIn>()?.OpenNpcWindow(npcStats.NpcWindow != NpcWindow.Undefined ? npcStats.NpcWindow : NpcWindow.Merchant);
                player.ViewPlugIns.GetPlugIn<IShowMerchantStoreItemListPlugIn>()?.ShowMerchantStoreItemList(npcStats.MerchantStore.Items, StoreKind.Normal);
            }
            else
            {
                this.ShowDialogOfOpenedNpc(player);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this action advances the player state to <see cref="PlayerState.NpcDialogOpened" />.
        /// </summary>
        /// <param name="npc">The NPC whose dialog is about to be opened.</param>
        /// <returns>A value indicating whether this action advances the player state to <see cref="PlayerState.NpcDialogOpened" />.</returns>
        protected virtual bool AdvancePlayerState(NonPlayerCharacter npc) => true;

        private void ShowDialogOfOpenedNpc(Player player)
        {
            var npcStats = player.OpenedNpc!.Definition;
            switch (npcStats.NpcWindow)
            {
                case NpcWindow.Undefined:
                    var eventArgs = new NpcTalkEventArgs();
                    player.GameContext.PlugInManager.GetPlugInPoint<IPlayerTalkToNpcPlugIn>()?.PlayerTalksToNpc(player, player.OpenedNpc, eventArgs);
                    if (!eventArgs.HasBeenHandled)
                    {
                        player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage($"Talking to this NPC ({npcStats.Number}, {npcStats.Designation}) is not implemented yet.", MessageType.BlueNormal);
                        player.PlayerState.TryAdvanceTo(PlayerState.EnteredWorld);
                    }
                    else if (!eventArgs.LeavesDialogOpen)
                    {
                        player.OpenedNpc = null;
                        player.PlayerState.TryAdvanceTo(PlayerState.EnteredWorld);
                    }
                    else
                    {
                        // Leaves dialog opened, so leave the state as it is.
                    }

                    break;
                case NpcWindow.VaultStorage:
                    player.Account!.Vault ??= player.PersistenceContext.CreateNew<ItemStorage>();
                    player.Vault = new Storage(InventoryConstants.WarehouseSize, player.Account.Vault);
                    player.ViewPlugIns.GetPlugIn<IShowVaultPlugIn>()?.ShowVault();
                    break;
                case NpcWindow.GuildMaster:
                    if (this.IsPlayedAllowedToCreateGuild(player))
                    {
                        player.ViewPlugIns.GetPlugIn<IShowGuildMasterDialogPlugIn>()?.ShowGuildMasterDialog();
                    }
                    else
                    {
                        player.OpenedNpc = null;
                        player.PlayerState.TryAdvanceTo(PlayerState.EnteredWorld);
                    }

                    break;
                case NpcWindow.LegacyQuest:
                    this.ShowLegacyQuestDialog(player);
                    break;
                default:
                    player.ViewPlugIns.GetPlugIn<IOpenNpcWindowPlugIn>()?.OpenNpcWindow(npcStats.NpcWindow);
                    break;
            }
        }

        private void ShowLegacyQuestDialog(Player player)
        {
            var quests = player.OpenedNpc!.Definition.Quests
                .Where(q => q.QualifiedCharacter is null || q.QualifiedCharacter == player.SelectedCharacter!.CharacterClass);

            if (!quests.Any())
            {
                player.ViewPlugIns.GetPlugIn<IShowMessageOfObjectPlugIn>()?.ShowMessageOfObject("I have no quests for you.", player.OpenedNpc);
                player.OpenedNpc = null;
                player.PlayerState.TryAdvanceTo(PlayerState.EnteredWorld);
                return;
            }

            if (quests.All(quest => quest.MinimumCharacterLevel > player.Level))
            {
                player.ViewPlugIns.GetPlugIn<IShowMessageOfObjectPlugIn>()?.ShowMessageOfObject(
                    "I have nothing to do for you. Come back with more power.", player.OpenedNpc);
                player.OpenedNpc = null;
                player.PlayerState.TryAdvanceTo(PlayerState.EnteredWorld);
                return;
            }

            var maxQuestNumber = quests.Max(q => q.Number);
            var questGroup = quests.FirstOrDefault(q => q.Number == maxQuestNumber)?.Group;
            var questState = player.GetQuestState(questGroup ?? 0);
            if (questState?.LastFinishedQuest?.Number >= maxQuestNumber)
            {
                player.ViewPlugIns.GetPlugIn<IShowMessageOfObjectPlugIn>()?.ShowMessageOfObject(
                    "I have nothing to do for you. You solved all my quests already.", player.OpenedNpc);
                player.OpenedNpc = null;
                player.PlayerState.TryAdvanceTo(PlayerState.EnteredWorld);
                return;
            }

            player.ViewPlugIns.GetPlugIn<ILegacyQuestStateDialogPlugIn>()?.Show();
        }

        private bool IsPlayedAllowedToCreateGuild(Player player)
        {
            if (player.Level < 100)
            {
                player.ViewPlugIns.GetPlugIn<IShowMessageOfObjectPlugIn>()?.ShowMessageOfObject("Your level should be at least level 100", player.OpenedNpc!);
                return false;
            }

            if (player.GuildStatus != null)
            {
                player.ViewPlugIns.GetPlugIn<IShowMessageOfObjectPlugIn>()?.ShowMessageOfObject("You already belong to a guild", player.OpenedNpc!);
                return false;
            }

            return true;
        }
    }
}
