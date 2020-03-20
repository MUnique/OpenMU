// <copyright file="TalkNpcAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions
{
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.NPC;
    using MUnique.OpenMU.GameLogic.PlugIns;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.Guild;
    using MUnique.OpenMU.GameLogic.Views.Inventory;
    using MUnique.OpenMU.GameLogic.Views.NPC;
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
            if (npcStats == null)
            {
                return;
            }

            if (!player.PlayerState.TryAdvanceTo(PlayerState.NpcDialogOpened))
            {
                return;
            }

            player.OpenedNpc = npc;
            if (npcStats.MerchantStore != null && npcStats.MerchantStore.Items.Count > 0)
            {
                player.ViewPlugIns.GetPlugIn<IOpenNpcWindowPlugIn>()?.OpenNpcWindow(npcStats.NpcWindow != NpcWindow.Undefined ? npcStats.NpcWindow : NpcWindow.Merchant);
                player.ViewPlugIns.GetPlugIn<IShowMerchantStoreItemListPlugIn>()?.ShowMerchantStoreItemList(npcStats.MerchantStore.Items);
            }
            else
            {
                this.ShowDialogOfOpenedNpc(player);
            }
        }

        private void ShowDialogOfOpenedNpc(Player player)
        {
            var npcStats = player.OpenedNpc.Definition;
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
                    player.Account.Vault ??= player.PersistenceContext.CreateNew<ItemStorage>();
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
                default:
                    player.ViewPlugIns.GetPlugIn<IOpenNpcWindowPlugIn>()?.OpenNpcWindow(npcStats.NpcWindow);
                    break;
            }
        }

        private bool IsPlayedAllowedToCreateGuild(Player player)
        {
            if (player.Level < 100)
            {
                player.ViewPlugIns.GetPlugIn<IShowMessageOfObjectPlugIn>()?.ShowMessageOfObject("Your level should be at least level 100", player.OpenedNpc);
                return false;
            }

            if (player.GuildStatus != null)
            {
                player.ViewPlugIns.GetPlugIn<IShowMessageOfObjectPlugIn>()?.ShowMessageOfObject("You already belong to a guild", player.OpenedNpc);
                return false;
            }

            return true;
        }
    }
}
