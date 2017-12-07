// <copyright file="MessengerView.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using GameLogic.Views;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// The default implementation of the mesenger view which is forwarding everything to the game client which specific data packets.
    /// </summary>
    public class MessengerView : IMessengerView
    {
        private readonly IConnection connection;

        private readonly Player player;

        private readonly IFriendServer friendServer;

        private readonly IAppearanceSerializer appearanceSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessengerView"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="player">The player.</param>
        /// <param name="friendServer">The friend server.</param>
        /// <param name="appearanceSerializer">The appearance serializer.</param>
        public MessengerView(IConnection connection, Player player, IFriendServer friendServer, IAppearanceSerializer appearanceSerializer)
        {
            this.connection = connection;
            this.player = player;
            this.friendServer = friendServer;
            this.appearanceSerializer = appearanceSerializer;
        }

        /// <inheritdoc/>
        public void InitializeMessenger(int maxLetters)
        {
            var letters = this.player.SelectedCharacter.Letters;
            var friends = this.friendServer.GetFriendList(this.player.SelectedCharacter.Id);
            var friendList = friends as ICollection<FriendViewItem> ?? friends.ToList();

            var letterCount = (byte)letters.Count;
            if (friendList.Count == 0)
            {
                this.connection.Send(new byte[] { 0xC2, 0, 7, 0xC0, letterCount, (byte)maxLetters, 0 });
                return;
            }

            var friendListCount = (byte)friendList.Count;
            const byte sizePerFriend = 11;
            ////C2 00 06 C0 06 32
            var packetLength = (ushort)(7 + (sizePerFriend * friendListCount));
            var packet = new byte[packetLength];
            packet[0] = 0xC2;
            packet[1] = packetLength.GetHighByte();
            packet[2] = packetLength.GetLowByte();
            packet[3] = 0xC0;
            packet[4] = letterCount;
            packet[5] = (byte)maxLetters;
            packet[6] = friendListCount;

            int i = 0;
            foreach (var friend in friendList)
            {
                var offset = 7 + (i * sizePerFriend);
                Encoding.ASCII.GetBytes(friend.FriendName, 0, friend.FriendName.Length, packet, offset);
                packet[offset + sizePerFriend - 1] = 0xFF;
                i++;
            }

            this.connection.Send(packet);

            foreach (var requesterName in this.friendServer.GetOpenFriendRequests(this.player.SelectedCharacter.Id))
            {
                this.ShowFriendRequest(requesterName);
            }
        }

        /// <inheritdoc/>
        public void AddToLetterList(LetterHeader letter, ushort newLetterIndex, bool newLetter)
        {
            var result = new byte[0x6C];
            result[0] = 0xC3;
            result[1] = (byte)result.Length;
            result[2] = 0xC6;
            ////4 + 5 == Id
            result[4] = (byte)(newLetterIndex & 0xFF);
            result[5] = (byte)((newLetterIndex >> 8) & 0xFF);
            Buffer.BlockCopy(Encoding.UTF8.GetBytes(letter.Sender.ToCharArray(), 0, letter.Sender.Length), 0, result, 6, Encoding.UTF8.GetByteCount(letter.Sender));
            var date = letter.LetterDate.ToUniversalTime().AddHours(this.player.Account.TimeZone).ToString(CultureInfo.InvariantCulture.DateTimeFormat);
            Buffer.BlockCopy(Encoding.UTF8.GetBytes(date.ToCharArray(), 0, date.Length), 0, result, 16, Encoding.UTF8.GetByteCount(date));
            Buffer.BlockCopy(Encoding.UTF8.GetBytes(letter.Subject.ToCharArray(), 0, letter.Subject.Length), 0, result, 46, Encoding.UTF8.GetByteCount(letter.Subject));
            result[78] = (byte)(letter.ReadFlag ? 1 : 0);
            if (result[78] == 1)
            {
                result[78] += (byte)(newLetter ? 1 : 0);
            }

            this.connection.Send(result);
        }

        /// <inheritdoc/>
        public void ShowFriendRequest(string requester)
        {
            var packet = new byte[] { 0xC1, 0x0D, 0xC2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            Encoding.UTF8.GetBytes(requester, 0, requester.Length, packet, 3);
            this.connection.Send(packet);
        }

        /// <inheritdoc/>
        public void FriendStateUpdate(string friend, int serverId)
        {
            var packet = new byte[] { 0xC1, 0x0E, 0xC4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, (byte)serverId };
            Encoding.UTF8.GetBytes(friend, 0, friend.Length, packet, 3);
            this.connection.Send(packet);
        }

        /// <inheritdoc/>
        public void FriendAdded(string friendname)
        {
            var packet = new byte[] { 0xC1, 0x0F, 0xC1, 0x01, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, (byte)SpecialServerId.Offline };
            Encoding.UTF8.GetBytes(friendname, 0, friendname.Length, packet, 4);
            this.connection.Send(packet);
        }

        /// <inheritdoc/>
        public void FriendDeleted(string deletingFriend)
        {
            var packet = new byte[] { 0xC1, 0x0E, 0xC3, 0x01, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            Encoding.UTF8.GetBytes(deletingFriend, 0, deletingFriend.Length, packet, 4);
            this.connection.Send(packet);
        }

        /// <inheritdoc/>
        public void ChatRoomCreated(ChatServerAuthenticationInfo authenticationInfo, string friendname, bool success)
        {
            var authenticationTokenArray = uint.Parse(authenticationInfo.AuthenticationToken).ToBytesSmallEndian();
            var chatRoomId = authenticationInfo.RoomId;
            var packet = new byte[]
            {
                0xC3, 0x24, 0xCA,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // chat server ip (without port)
                chatRoomId.GetLowByte(), chatRoomId.GetHighByte(),
                authenticationTokenArray[0], authenticationTokenArray[1], authenticationTokenArray[2], authenticationTokenArray[3],
                0x01, // type
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // friendname
                success ? (byte)1 : (byte)0
            };

            // chatserver unavailable would have success = 2, type = 0xAF
            var chatServerIp = this.friendServer.GetChatserverIP();
            Encoding.ASCII.GetBytes(chatServerIp, 0, chatServerIp.Length, packet, 3);
            Encoding.UTF8.GetBytes(friendname, 0, friendname.Length, packet, 25);
            this.connection.Send(packet);
        }

        /// <inheritdoc/>
        public void ShowLetter(LetterBody letter)
        {
            var letterIndex = this.player.SelectedCharacter.Letters.IndexOf(letter.Header);
            var len = (ushort)(Encoding.UTF8.GetByteCount(letter.Message) + 28);
            var result = new byte[len];
            result[0] = 0xC4;
            result[1] = len.GetHighByte();
            result[2] = len.GetLowByte();
            result[3] = 0xC7;
            result[4] = ((ushort)letterIndex).GetLowByte();
            result[5] = ((ushort)letterIndex).GetHighByte();
            result[6] = ((ushort)(len - 28)).GetLowByte();
            result[7] = ((ushort)(len - 28)).GetHighByte();
            var appearanceBytes = this.appearanceSerializer.GetAppearanceData(letter.SenderAppearance);
            Buffer.BlockCopy(appearanceBytes, 0, result, 8, appearanceBytes.Length);
            result[8 + appearanceBytes.Length] = letter.Rotation; // TODO: check if the index is correct
            result[9 + appearanceBytes.Length] = letter.Animation;
            Buffer.BlockCopy(Encoding.UTF8.GetBytes(letter.Message), 0, result, 28, Encoding.UTF8.GetByteCount(letter.Message));

            this.connection.Send(result);
        }

        /// <inheritdoc/>
        public void LetterDeleted(ushort letterIndex)
        {
            this.connection.Send(new byte[] { 0xC1, 6, 0xC8, 1, letterIndex.GetLowByte(), letterIndex.GetHighByte() });
        }

        /// <inheritdoc/>
        public void LetterSendResult(LetterSendSuccess success)
        {
            byte length = 1; // TODO
            this.connection.Send(new byte[] { 0xC1, 8, 0xC5, (byte)success, length, 0, 0, 0 });
        }
    }
}
