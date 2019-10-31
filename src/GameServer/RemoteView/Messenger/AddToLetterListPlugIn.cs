// <copyright file="AddToLetterListPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Messenger
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.Messenger;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IAddToLetterListPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("AddToLetterListPlugIn", "The default implementation of the IAddToLetterListPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("d30bea99-9d77-4182-99be-e08095c1969f")]
    public class AddToLetterListPlugIn : IAddToLetterListPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddToLetterListPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public AddToLetterListPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void AddToLetterList(LetterHeader letter, ushort newLetterIndex, bool newLetter)
        {
            using var writer = this.player.Connection.StartSafeWrite(AddLetter.HeaderType, AddLetter.Length);
            _ = new AddLetter(writer.Span)
            {
                LetterIndex = newLetterIndex,
                SenderName = letter.SenderName,
                Subject = letter.Subject,
                Timestamp = letter.LetterDate.ToUniversalTime().AddHours(this.player.Account.TimeZone).ToString("yyyy-MM-dd HH:mm:ss"),
                State = newLetter ? AddLetter.LetterState.New : letter.ReadFlag ? AddLetter.LetterState.Read : AddLetter.LetterState.Unread,
            };

            writer.Commit();
        }
    }
}