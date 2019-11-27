// <copyright file="FriendAddedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Messenger
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.Messenger;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IFriendAddedPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("FriendAddedPlugIn", "The default implementation of the IFriendAddedPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("7edba5ed-eec5-4aa7-b302-418444868841")]
    public class FriendAddedPlugIn : IFriendAddedPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="FriendAddedPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public FriendAddedPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void FriendAdded(string friendName)
        {
            using var writer = this.player.Connection.StartSafeWrite(
                Network.Packets.ServerToClient.FriendAdded.HeaderType,
                Network.Packets.ServerToClient.FriendAdded.Length);
            _ = new FriendAdded(writer.Span)
            {
                FriendName = friendName,
            };

            writer.Commit();
        }
    }
}