// <copyright file="ShowMessagePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView
{
    using System.Runtime.InteropServices;
    using System.Text;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IShowMessagePlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ShowMessagePlugIn", "The default implementation of the IShowMessagePlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("e294f4ce-f2c6-4a92-8cd0-40d8d5afae66")]
    public class ShowMessagePlugIn : IShowMessagePlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowMessagePlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ShowMessagePlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void ShowMessage(string message, OpenMU.Interfaces.MessageType messageType)
        {
            const string messagePrefix = "000000000";
            string content = messagePrefix + message;
            using (var writer = this.player.Connection.StartSafeWrite(0xC1, 5 + content.Length))
            {
                var packet = writer.Span;
                packet[2] = 13;
                packet[3] = (byte)messageType;
                packet.Slice(4).WriteString(content, Encoding.UTF8);
                writer.Commit();
            }
        }
    }
}