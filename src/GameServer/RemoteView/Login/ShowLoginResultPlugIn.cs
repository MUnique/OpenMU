// <copyright file="ShowLoginResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Login
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.Login;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IShowLoginResultPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ShowLoginResultPlugIn", "The default implementation of the IShowLoginResultPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("9ba2646b-72c2-4876-a316-c9aadb386037")]
    public class ShowLoginResultPlugIn : IShowLoginResultPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowLoginResultPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ShowLoginResultPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void ShowLoginResult(LoginResult loginResult)
        {
            using (var writer = this.player.Connection.StartSafeWrite(0xC1, 0x05))
            {
                var packet = writer.Span;
                packet[2] = 0xF1;
                packet[3] = 0x01;
                packet[4] = (byte)loginResult;
                writer.Commit();
            }
        }
    }
}