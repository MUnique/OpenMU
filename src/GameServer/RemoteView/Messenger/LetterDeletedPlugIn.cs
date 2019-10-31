// <copyright file="LetterDeletedPlugIn.cs" company="MUnique">
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
    /// The default implementation of the <see cref="ILetterDeletedPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("LetterDeletedPlugIn", "The default implementation of the ILetterDeletedPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("3bcd507e-cb61-4b25-a208-4ee264f4793e")]
    public class LetterDeletedPlugIn : ILetterDeletedPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="LetterDeletedPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public LetterDeletedPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void LetterDeleted(ushort letterIndex)
        {
            using var writer = this.player.Connection.StartSafeWrite(RemoveLetter.HeaderType, RemoveLetter.Length);
            _ = new RemoveLetter(writer.Span)
            {
                LetterIndex = letterIndex,
            };
            writer.Commit();
        }
    }
}