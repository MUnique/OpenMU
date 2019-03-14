﻿// <copyright file="MessengerView.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the messenger view which is forwarding everything to the game client which specific data packets.
    /// </summary>
    [PlugIn("Messenger View", "The default implementation of the messenger view which is forwarding everything to the game client which specific data packets.")]
    [Guid("2C98E3F9-E7B3-4BF9-9E1F-FFC3702D4211")]
    public class MessengerView : IMessengerView
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessengerView"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public MessengerView(RemotePlayer player)
        {
            this.player = player;
        }

        private IConnection Connection => this.player.Connection;

        /// <inheritdoc/>
        public void InitializeMessenger(int maxLetters)
        {
            var friendServer = this.player.GameServerContext.FriendServer;
            var friends = friendServer.GetFriendList(this.player.SelectedCharacter.Id);
            var friendList = friends as ICollection<string> ?? friends.ToList();

            var letterCount = (byte)this.player.SelectedCharacter.Letters.Count;
            if (friendList.Count == 0)
            {
                using (var writer = this.Connection.StartSafeWrite(0xC2, 7))
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
                using (var writer = this.Connection.StartSafeWrite(0xC2, packetLength))
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
                this.ShowFriendRequest(requesterName);
            }

            var letters = this.player.SelectedCharacter.Letters;
            for (ushort l = 0; l < letters.Count; l++)
            {
                this.AddToLetterList(letters[l], l, false);
            }
        }

        /// <inheritdoc/>
        public void AddToLetterList(LetterHeader letter, ushort newLetterIndex, bool newLetter)
        {
            using (var writer = this.Connection.StartSafeWrite(0xC3, 79))
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

        /// <inheritdoc/>
        public void ShowFriendRequest(string requester)
        {
            using (var writer = this.Connection.StartSafeWrite(0xC1, 0x0D))
            {
                var packet = writer.Span;
                packet[2] = 0xC2;
                packet.Slice(3).WriteString(requester, Encoding.UTF8);
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void FriendStateUpdate(string friend, int serverId)
        {
            using (var writer = this.Connection.StartSafeWrite(0xC1, 0x0E))
            {
                var packet = writer.Span;
                packet[2] = 0xC4;
                packet.Slice(3).WriteString(friend, Encoding.UTF8);
                packet[packet.Length - 1] = (byte)serverId;
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void FriendAdded(string friendName)
        {
            using (var writer = this.Connection.StartSafeWrite(0xC1, 0x0F))
            {
                var packet = writer.Span;
                packet[2] = 0xC1;
                packet[3] = 0x01;
                packet.Slice(4).WriteString(friendName, Encoding.UTF8);
                packet[packet.Length - 1] = (byte)SpecialServerId.Offline;
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void FriendDeleted(string deletingFriend)
        {
            using (var writer = this.Connection.StartSafeWrite(0xC1, 0x0E))
            {
                var packet = writer.Span;
                packet[2] = 0xC3;
                packet[3] = 0x01;
                packet.Slice(4).WriteString(deletingFriend, Encoding.UTF8);
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void ChatRoomCreated(ChatServerAuthenticationInfo authenticationInfo, string friendName, bool success)
        {
            using (var writer = this.Connection.StartSafeWrite(0xC3, 0x24))
            {
                var chatServerIp = this.player.GameServerContext.FriendServer.GetChatserverIP();
                var packet = writer.Span;
                packet[2] = 0xCA;
                packet.Slice(3, 15).WriteString(chatServerIp, Encoding.UTF8);
                var chatRoomId = authenticationInfo.RoomId;
                packet[18] = chatRoomId.GetLowByte();
                packet[19] = chatRoomId.GetHighByte();
                packet.Slice(20, 4).SetIntegerBigEndian(uint.Parse(authenticationInfo.AuthenticationToken));

                packet[24] = 0x01; // type
                packet.Slice(25, 10).WriteString(friendName, Encoding.UTF8);
                packet[35] = success ? (byte)1 : (byte)0;
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void ShowFriendInvitationResult(bool success, uint requestId)
        {
            using (var writer = this.Connection.StartSafeWrite(0xC3, 8))
            {
                var packet = writer.Span;
                packet[2] = 0xCB;
                packet[3] = success ? (byte)1 : (byte)0;
                packet.Slice(4, 4).SetIntegerSmallEndian(requestId);
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void ShowLetter(LetterBody letter)
        {
            var appearanceSerializer = this.player.AppearanceSerializer;
            var letterIndex = this.player.SelectedCharacter.Letters.IndexOf(letter.Header);
            var headerSize = appearanceSerializer.NeededSpace + 10;
            var messageSize = Encoding.UTF8.GetByteCount(letter.Message);
            var len = (ushort)(messageSize + headerSize);
            using (var writer = this.Connection.StartSafeWrite(0xC4, len))
            {
                var result = writer.Span;
                result[3] = 0xC7;
                result[4] = ((ushort)letterIndex).GetLowByte();
                result[5] = ((ushort)letterIndex).GetHighByte();
                result[6] = ((ushort)messageSize).GetLowByte();
                result[7] = ((ushort)messageSize).GetHighByte();
                appearanceSerializer.WriteAppearanceData(result.Slice(8, appearanceSerializer.NeededSpace), letter.SenderAppearance, false);
                result[8 + appearanceSerializer.NeededSpace] = letter.Rotation;
                result[9 + appearanceSerializer.NeededSpace] = letter.Animation;
                result.Slice(headerSize).WriteString(letter.Message, Encoding.UTF8);
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void LetterDeleted(ushort letterIndex)
        {
            using (var writer = this.Connection.StartSafeWrite(0xC1, 6))
            {
                var packet = writer.Span;
                packet[2] = 0xC8;
                packet[3] = 1;
                packet[4] = letterIndex.GetLowByte();
                packet[5] = letterIndex.GetHighByte();
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void LetterSendResult(LetterSendSuccess success, uint letterId)
        {
            using (var writer = this.Connection.StartSafeWrite(0xC1, 8))
            {
                var packet = writer.Span;
                packet[2] = 0xC5;
                packet[3] = (byte)success;
                packet.Slice(4, 4).SetIntegerBigEndian(letterId);
                writer.Commit();
            }
        }
    }
}
