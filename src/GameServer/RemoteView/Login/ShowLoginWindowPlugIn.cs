// <copyright file="ShowLoginWindowPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Login
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.Login;
    using MUnique.OpenMU.GameServer.MessageHandler.Login;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IShowLoginWindowPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ShowLoginWindowPlugIn", "The default implementation of the IShowLoginWindowPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("c5240952-1870-4f09-a3e4-9f6413845a23")]
    [Client(6, 3, ClientLanguage.English)]
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
            using (var writer = this.player.Connection.StartSafeWrite(0xC1, 0x0C))
            {
                var message = writer.Span;
                message[2] = 0xF1;
                message[3] = 0x00;
                message[4] = 0x01; // Success
                message[5] = ViewExtensions.ConstantPlayerId.GetHighByte();
                message[6] = ViewExtensions.ConstantPlayerId.GetLowByte();
                ClientVersionResolver.Resolve(this.player.ClientVersion).CopyTo(message.Slice(7, 5));
                writer.Commit();
            }
        }
    }
}