// <copyright file="ShowVaultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Inventory
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.Views.Inventory;
    using MUnique.OpenMU.GameLogic.Views.NPC;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IShowVaultPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ShowVaultPlugIn", "The default implementation of the IShowVaultPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("aa20c7aa-08ad-4fec-9138-88bcdc690afa")]
    public class ShowVaultPlugIn : IShowVaultPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowVaultPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ShowVaultPlugIn(RemotePlayer player) => this.player = player;

        private enum VaultState : byte
        {
            Unprotected = 0,
            Locked = 1,
            UnlockFailedByWrongPin = 10,
            SetPinFailedBecauseLock = 11,
            Unlocked = 12,
            RemovePinFailedByWrongPassword = 13,
        }

        /// <inheritdoc/>
        public void ShowVault()
        {
            this.player.ViewPlugIns.GetPlugIn<IOpenNpcWindowPlugIn>()?.OpenNpcWindow(NpcWindow.VaultStorage);
            var itemSerializer = this.player.ItemSerializer;
            var itemsCount = this.player.Vault.ItemStorage.Items.Count;
            var spacePerItem = itemSerializer.NeededSpace + 1;
            using (var writer = this.player.Connection.StartSafeWrite(0xC2, 6 + (itemsCount * spacePerItem)))
            {
                var packet = writer.Span;
                packet[3] = 0x31;
                packet[4] = 0;
                packet[5] = (byte)itemsCount;
                int i = 0;
                foreach (var item in this.player.Vault.Items)
                {
                    var itemBlock = packet.Slice(6 + (i * spacePerItem), spacePerItem);
                    itemBlock[0] = item.ItemSlot;
                    itemSerializer.SerializeItem(itemBlock.Slice(1), item);
                    i++;
                }

                writer.Commit();
            }

            this.player.ViewPlugIns.GetPlugIn<IUpdateVaultMoneyPlugIn>()?.UpdateVaultMoney();

            using (var writer = this.player.Connection.StartSafeWrite(0xC1, 4))
            {
                // vault password protection info
                var packet = writer.Span;
                packet[2] = 0x83;
                packet[3] = (byte)VaultState.Unprotected;
                writer.Commit();
            }
        }
    }
}