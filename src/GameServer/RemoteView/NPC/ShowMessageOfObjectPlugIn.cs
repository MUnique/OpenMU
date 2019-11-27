// <copyright file="ShowMessageOfObjectPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.NPC
{
    using System.Runtime.InteropServices;
    using System.Text;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views.NPC;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IShowMessageOfObjectPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ShowMessageOfObjectPlugIn", "The default implementation of the IShowMessageOfObjectPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("41d28cd1-1fb7-4af7-b635-8af9923351bd")]
    public class ShowMessageOfObjectPlugIn : IShowMessageOfObjectPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowMessageOfObjectPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ShowMessageOfObjectPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc />
        public void ShowMessageOfObject(string message, IIdentifiable sender)
        {
            using var writer = this.player.Connection.StartSafeWrite(ObjectMessage.HeaderType, ObjectMessage.GetRequiredSize(message));
            _ = new ObjectMessage(writer.Span)
            {
                ObjectId = sender.Id,
                Message = message,
            };
            writer.Commit();
        }
    }
}