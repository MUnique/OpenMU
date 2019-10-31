// <copyright file="ShowLoginWindowPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Login
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.Login;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IShowLoginWindowPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ShowLoginWindowPlugIn", "The default implementation of the IShowLoginWindowPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("c5240952-1870-4f09-a3e4-9f6413845a23")]
    public class ShowLoginWindowPlugIn : IShowLoginWindowPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowLoginWindowPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ShowLoginWindowPlugIn(RemotePlayer player)
        {
            this.player = player;
        }

        /// <inheritdoc/>
        public void ShowLoginWindow()
        {
            using var writer = this.player.Connection.StartSafeWrite(GameServerEntered.HeaderType, GameServerEntered.Length);
            var message = new GameServerEntered(writer.Span)
            {
                PlayerId = ViewExtensions.ConstantPlayerId,
            };

            ClientVersionResolver.Resolve(this.player.ClientVersion).CopyTo(message.Version);
            writer.Commit();
        }
    }
}