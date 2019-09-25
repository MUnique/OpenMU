// <copyright file="AddToLetterListPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Messenger
{
    using System.Runtime.InteropServices;
    using System.Text;
    using MUnique.OpenMU.GameLogic.Views.Messenger;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets;
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
            using (var writer = this.player.Connection.StartSafeWrite(0xC3, 79))
            {
                var result = writer.Span;
                result[2] = 0xC6;
                result[4] = newLetterIndex.GetLowByte();
                result[5] = newLetterIndex.GetHighByte();
                result.Slice(6, 10).WriteString(letter.SenderName, Encoding.UTF8);
                var date = letter.LetterDate.ToUniversalTime().AddHours(this.player.Account.TimeZone).ToString("yyyy-MM-dd HH:mm:ss");
                result.Slice(16, 30).WriteString(date, Encoding.UTF8);
                result.Slice(46, 32).WriteString(letter.Subject, Encoding.UTF8);
                result[78] = (byte)(letter.ReadFlag ? 1 : 0);
                if (result[78] == 1)
                {
                    result[78] += (byte)(newLetter ? 1 : 0);
                }

                writer.Commit();
            }
        }
    }
}