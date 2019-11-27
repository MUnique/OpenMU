// <copyright file="ShowGuildCreationDialogPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Guild
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.Guild;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IShowGuildCreationDialogPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ShowGuildCreationDialogPlugIn", "The default implementation of the IShowGuildCreationDialogPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("ed6fbe5f-7a27-477d-b238-e6e77cf113d8")]
    public class ShowGuildCreationDialogPlugIn : IShowGuildCreationDialogPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowGuildCreationDialogPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ShowGuildCreationDialogPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void ShowGuildCreationDialog()
        {
            using var writer = this.player.Connection.StartSafeWrite(Network.Packets.ServerToClient.ShowGuildCreationDialog.HeaderType, Network.Packets.ServerToClient.ShowGuildCreationDialog.Length);
            _ = new ShowGuildCreationDialog(writer.Span);
            writer.Commit();
        }
    }
}