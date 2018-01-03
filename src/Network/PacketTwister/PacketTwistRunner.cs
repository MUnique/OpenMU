// <copyright file="PacketTwistRunner.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The main entry point to decrypt packets with the packet twisting algorithm.
    /// </summary>
    public class PacketTwistRunner : IEncryptor, IDecryptor
    {
        private readonly IDictionary<byte, IPacketTwister> twisters = new Dictionary<byte, IPacketTwister>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketTwistRunner"/> class.
        /// </summary>
        public PacketTwistRunner()
        {
            this.twisters.Add(0x0E, new PacketTwisterOfPing());
            this.twisters.Add(0xF1, new PacketTwisterOfLoginLogout());
            this.twisters.Add(0xF3, new PacketTwisterOfCharacterManagement());
            this.twisters.Add(0x00, new PacketTwisterOfChat());
            this.twisters.Add(0x01, new PacketTwister5());
            this.twisters.Add(0x02, new PacketTwisterOfWhisper());
            this.twisters.Add(0x03, new PacketTwisterOfClientChecksum());
            this.twisters.Add(0x18, new PacketTwisterOfAnimation());
            this.twisters.Add(0x19, new PacketTwisterOfSkill());
            this.twisters.Add(0x22, new PacketTwisterOfItemPickup());
            this.twisters.Add(0x23, new PacketTwisterOfItemDrop());
            this.twisters.Add(0x24, new PacketTwisterOfItemMove());
            this.twisters.Add(0x26, new PacketTwisterOfItemConsume());
            this.twisters.Add(0x30, new PacketTwisterOfNpcTalkRequest());
            this.twisters.Add(0x31, new PacketTwisterOfNpcDialogClose());
            this.twisters.Add(0x32, new PacketTwisterOfNpcItemBuyRequest());
            this.twisters.Add(0x33, new PacketTwisterOfNpcItemSellRequest());
            this.twisters.Add(0x34, new PacketTwisterOfItemRepairRequest());
            this.twisters.Add(0x36, new PacketTwisterOfTradeRequest());
            this.twisters.Add(0x37, new PacketTwisterOfTradeResponse());
            this.twisters.Add(0x3A, new PacketTwisterOfTradeMoney());
            this.twisters.Add(0x3C, new PacketTwisterOfTradeButton());
            this.twisters.Add(0x3D, new PacketTwisterOfTradeCancel());
            this.twisters.Add(0x3F, new PacketTwisterOfPersonalShop());
            this.twisters.Add(0x40, new PacketTwisterOfPartyRequest());
            this.twisters.Add(0x41, new PacketTwisterOfPartyRequestAnswer());
            this.twisters.Add(0x42, new PacketTwisterOfPartyList());
            this.twisters.Add(0x43, new PacketTwisterOfPartyKick());
            this.twisters.Add(0x4A, new PacketTwister29());
            this.twisters.Add(0x4B, new PacketTwister30());
            this.twisters.Add(0x50, new PacketTwisterOfGuildJoinRequest());
            this.twisters.Add(0x51, new PacketTwisterOfGuildJoinResponse());
            this.twisters.Add(0x52, new PacketTwisterOfGuildList());
            this.twisters.Add(0x53, new PacketTwisterOfGuildKick());
            //// this.twisters.Add(0x54, new PacketTwisterOfGuildMasterResponse()); // implementation seems to be wrong
            this.twisters.Add(0x55, new PacketTwisterOfGuildMasterInfo());
            this.twisters.Add(0x57, new PacketTwister37());
            this.twisters.Add(0x61, new PacketTwisterOfGuildWarRequest());
            this.twisters.Add(0x66, new PacketTwisterOfGuildInfo());
            this.twisters.Add(0xB1, new PacketTwisterOfCastleSiege());
            this.twisters.Add(0xB2, new PacketTwister41());
            this.twisters.Add(0xB3, new PacketTwister42());
            this.twisters.Add(0xB7, new PacketTwisterOfCastleSiegeWeapon());
            this.twisters.Add(0xBD, new PacketTwisterOfCrywolf());
            this.twisters.Add(0xE2, new PacketTwisterOfGuildAssign());
            this.twisters.Add(0xE5, new PacketTwisterOfAllianceJoinRequest());
            this.twisters.Add(0x71, new PacketTwister47());
            this.twisters.Add(0x72, new PacketTwister48());
            this.twisters.Add(0x73, new PacketTwisterOfGameGuardAuth());
            this.twisters.Add(0x81, new PacketTwisterOfVaultMoney());
            this.twisters.Add(0x82, new PacketTwisterOfVaultClose());
            this.twisters.Add(0x83, new PacketTwisterOfVaultPassword());
            this.twisters.Add(0x86, new PacketTwisterOfChaosMachineMix());
            this.twisters.Add(0x87, new PacketTwisterOfChaosMachineClose());
            this.twisters.Add(0x90, new PacketTwisterOfDevilSquareEnter());
            this.twisters.Add(0x91, new PacketTwisterDevilSquareRemainingTime());
            this.twisters.Add(0x95, new PacketTwisterOfEventChipRegistration());
            this.twisters.Add(0x96, new PacketTwister58());
            this.twisters.Add(0x97, new PacketTwister59());
            this.twisters.Add(0xA0, new PacketTwisterOfQuestInfoRequest());
            this.twisters.Add(0xA2, new PacketTwisterOfQuestState());
            this.twisters.Add(0xA7, new PacketTwisterOfPetItemCommand());
            this.twisters.Add(0xA9, new PacketTwisterOfPetItemInfoRequest());
            this.twisters.Add(0x9A, new PacketTwisterOfBloodCastleEnterRequest());
            this.twisters.Add(0x9F, new PacketTwisterOfBloodCastleInfoRequest());
            this.twisters.Add(0x9D, new PacketTwisterOfLottoRegistration());
            this.twisters.Add(0xC0, new PacketTwisterOfFriendList());
            this.twisters.Add(0xC1, new PacketTwisterOfFriendAddRequest());
            this.twisters.Add(0xC2, new PacketTwisterOfFriendAddResponse());
            this.twisters.Add(0xC3, new PacketTwisterOfFriendDeleteRequest());
            this.twisters.Add(0xC4, new PacketTwisterOfFriendStateChange());
            this.twisters.Add(0xC5, new PacketTwisterOfLetterSendRequest());
            this.twisters.Add(0xC7, new PacketTwisterOfLetterReadRequest());
            this.twisters.Add(0xC8, new PacketTwisterOfLetterDeleteRequest());
            this.twisters.Add(0xC9, new PacketTwisterOfLetterListRequest());
            this.twisters.Add(0xCA, new PacketTwisterOfChatRoomCreateRequest());
            this.twisters.Add(0xCB, new PacketTwisterOfChatInvitationRequest());
            this.twisters.Add(0xF7, new PacketTwister78());
            this.twisters.Add(0xF6, new PacketTwister79());
            this.twisters.Add(0xD2, new PacketTwisterOfCashShop());
            this.twisters.Add(0xFA, new PacketTwister81());
        }

        /// <inheritdoc/>
        public byte[] Encrypt(byte[] packet)
        {
            var packetType = packet.GetPacketType();
            var data = new ArraySegment<byte>(packet, packet.GetPacketHeaderSize() + 1);
            if (this.twisters.TryGetValue(packetType, out IPacketTwister twister))
            {
                twister.Twist(data);
            }

            return packet;
        }

        /// <inheritdoc/>
        public bool Decrypt(ref byte[] packet)
        {
            var packetType = packet.GetPacketType();
            var data = new ArraySegment<byte>(packet, packet.GetPacketHeaderSize() + 1);
            if (this.twisters.TryGetValue(packetType, out IPacketTwister twister))
            {
                twister.Correct(data);
            }

            return true;
        }

        /// <inheritdoc/>
        public void Reset()
        {
            // not required
        }

        private struct ArraySegment<T> : IList<T>
        {
            private readonly IList<T> innerArray;
            private readonly int startIndex;

            public ArraySegment(IList<T> array, int startIndex)
            {
                this.innerArray = array;
                this.startIndex = startIndex;
            }

            public int Count => this.innerArray.Count - this.startIndex;

            public bool IsReadOnly => true;

            public T this[int index]
            {
                get => this.innerArray[index + this.startIndex];
                set => this.innerArray[index + this.startIndex] = value;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return this.innerArray.Skip(this.startIndex).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            public void Add(T item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(T item)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public bool Remove(T item)
            {
                throw new NotImplementedException();
            }

            public int IndexOf(T item)
            {
                return this.innerArray.IndexOf(item) - this.startIndex;
            }

            public void Insert(int index, T item)
            {
                throw new NotImplementedException();
            }

            public void RemoveAt(int index)
            {
                throw new NotImplementedException();
            }
        }
    }
}