// <copyright file="InitializeMessengerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Messenger
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using MUnique.OpenMU.GameLogic.Views.Messenger;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IInitializeMessengerPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("InitializeMessengerPlugIn", "The default implementation of the IInitializeMessengerPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("cb079c23-e90c-473d-87ae-317937158924")]
    public class InitializeMessengerPlugIn : IInitializeMessengerPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="InitializeMessengerPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public InitializeMessengerPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void InitializeMessenger(int maxLetters)
        {
            var friendServer = this.player.GameServerContext.FriendServer;
            var friends = friendServer.GetFriendList(this.player.SelectedCharacter.Id);
            var friendList = friends as ICollection<string> ?? friends.ToList();

            var letterCount = (byte)this.player.SelectedCharacter.Letters.Count;
            if (friendList.Count == 0)
            {
                using (var writer = this.player.Connection.StartSafeWrite(0xC2, 7))
                {
                    var packet = writer.Span;
                    packet[3] = 0xC0;
                    packet[4] = letterCount;
                    packet[5] = (byte)maxLetters;
                    packet[6] = 0;
                    writer.Commit();
                }
            }
            else
            {
                var friendListCount = (byte)friendList.Count;
                const byte sizePerFriend = 11;
                ////C2 00 06 C0 06 32
                var packetLength = (ushort)(7 + (sizePerFriend * friendListCount));
                using (var writer = this.player.Connection.StartSafeWrite(0xC2, packetLength))
                {
                    var packet = writer.Span;
                    packet[3] = 0xC0;
                    packet[4] = letterCount;
                    packet[5] = (byte)maxLetters;
                    packet[6] = friendListCount;

                    int i = 0;
                    foreach (var friend in friendList)
                    {
                        var friendBlock = packet.Slice(7 + (i * sizePerFriend), sizePerFriend);
                        friendBlock.WriteString(friend, Encoding.UTF8);
                        friendBlock[friendBlock.Length - 1] = 0xFF;
                        i++;
                    }

                    writer.Commit();
                }
            }

            foreach (var requesterName in friendServer.GetOpenFriendRequests(this.player.SelectedCharacter.Id))
            {
                this.player.ViewPlugIns.GetPlugIn<IShowFriendRequestPlugIn>()?.ShowFriendRequest(requesterName);
            }

            var letters = this.player.SelectedCharacter.Letters;
            for (ushort l = 0; l < letters.Count; l++)
            {
                this.player.ViewPlugIns.GetPlugIn<IAddToLetterListPlugIn>()?.AddToLetterList(letters[l], l, false);
            }
        }
    }
}
