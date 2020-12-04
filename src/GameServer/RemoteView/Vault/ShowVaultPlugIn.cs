// <copyright file="ShowVaultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Vault
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.Views.NPC;
    using MUnique.OpenMU.GameLogic.Views.Vault;
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

        /// <inheritdoc/>
        public void ShowVault()
        {
            this.player.ViewPlugIns.GetPlugIn<IOpenNpcWindowPlugIn>()?.OpenNpcWindow(NpcWindow.VaultStorage);
            this.player.ViewPlugIns.GetPlugIn<IShowMerchantStoreItemListPlugIn>()?.ShowMerchantStoreItemList(this.player.Vault.ItemStorage.Items, StoreKind.Normal);
            this.player.ViewPlugIns.GetPlugIn<IUpdateVaultMoneyPlugIn>()?.UpdateVaultMoney(true);
            this.player.ViewPlugIns.GetPlugIn<IUpdateVaultStatePlugIn>()?.UpdateState();
        }
    }
}