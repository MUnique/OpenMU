// <copyright file="TalkNpcAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions
{
    using Interfaces;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.NPC;
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
                player.PlayerView.OpenNpcWindow(npcStats.NpcWindow != NpcWindow.Undefined ? npcStats.NpcWindow : NpcWindow.Merchant);
                player.PlayerView.ShowMerchantStoreItemList(npcStats.MerchantStore.Items);
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
                    player.PlayerView.ShowMessage($"Talking to this NPC ({npcStats.Number}, {npcStats.Designation}) is not implemented yet.", MessageType.BlueNormal);
                    player.PlayerState.TryAdvanceTo(PlayerState.EnteredWorld);
                    break;
                case NpcWindow.VaultStorage:
                    player.Vault = new Storage(0, 0, InventoryConstants.WarehouseSize, player.Account.Vault);
                    player.PlayerView.ShowVault();
                    break;
                case NpcWindow.GuildMaster:
                    if (this.IsPlayedAllowedToCreateGuild(player))
                    {
                        player.PlayerView.GuildView.ShowGuildMasterDialog();
                    }
                    else
                    {
                        player.OpenedNpc = null;
                        player.PlayerState.TryAdvanceTo(PlayerState.EnteredWorld);
                    }

                    break;
                default:
                    player.PlayerView.OpenNpcWindow(npcStats.NpcWindow);
                    break;
            }
        }

        private bool IsPlayedAllowedToCreateGuild(Player player)
        {
            if (player.Level < 100)
            {
                player.PlayerView.ShowMessageOfObject("Your level should be at least level 100", player.OpenedNpc);
                return false;
            }

            if (player.GuildStatus != null)
            {
                player.PlayerView.ShowMessageOfObject("You already belong to a guild", player.OpenedNpc);
                return false;
            }

            return true;
        }
    }
}
