// <copyright file="PacketTwistRunner.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister;

/// <summary>
/// The main entry point to decrypt packets with the packet twisting algorithm.
/// </summary>
public class PacketTwistRunner : ISpanDecryptor, ISpanEncryptor
{
    private readonly IDictionary<byte, IPacketTwister> _twisters = new Dictionary<byte, IPacketTwister>();

    /// <summary>
    /// Initializes a new instance of the <see cref="PacketTwistRunner"/> class.
    /// </summary>
    public PacketTwistRunner()
    {
        this._twisters.Add(0x0E, new PacketTwisterOfPing());
        this._twisters.Add(0xF1, new PacketTwisterOfLoginLogout());
        this._twisters.Add(0xF3, new PacketTwisterOfCharacterManagement());
        this._twisters.Add(0x00, new PacketTwisterOfChat());
        this._twisters.Add(0x01, new PacketTwister5());
        this._twisters.Add(0x02, new PacketTwisterOfWhisper());
        this._twisters.Add(0x03, new PacketTwisterOfClientChecksum());
        this._twisters.Add(0x18, new PacketTwisterOfAnimation());
        this._twisters.Add(0x19, new PacketTwisterOfSkill());
        this._twisters.Add(0x22, new PacketTwisterOfItemPickup());
        this._twisters.Add(0x23, new PacketTwisterOfItemDrop());
        this._twisters.Add(0x24, new PacketTwisterOfItemMove());
        this._twisters.Add(0x26, new PacketTwisterOfItemConsume());
        this._twisters.Add(0x30, new PacketTwisterOfNpcTalkRequest());
        this._twisters.Add(0x31, new PacketTwisterOfNpcDialogClose());
        this._twisters.Add(0x32, new PacketTwisterOfNpcItemBuyRequest());
        this._twisters.Add(0x33, new PacketTwisterOfNpcItemSellRequest());
        this._twisters.Add(0x34, new PacketTwisterOfItemRepairRequest());
        this._twisters.Add(0x36, new PacketTwisterOfTradeRequest());
        this._twisters.Add(0x37, new PacketTwisterOfTradeResponse());
        this._twisters.Add(0x3A, new PacketTwisterOfTradeMoney());
        this._twisters.Add(0x3C, new PacketTwisterOfTradeButton());
        this._twisters.Add(0x3D, new PacketTwisterOfTradeCancel());
        this._twisters.Add(0x3F, new PacketTwisterOfPersonalShop());
        this._twisters.Add(0x40, new PacketTwisterOfPartyRequest());
        this._twisters.Add(0x41, new PacketTwisterOfPartyRequestAnswer());
        this._twisters.Add(0x42, new PacketTwisterOfPartyList());
        this._twisters.Add(0x43, new PacketTwisterOfPartyKick());
        this._twisters.Add(0x4A, new PacketTwister29());
        this._twisters.Add(0x4B, new PacketTwister30());
        this._twisters.Add(0x50, new PacketTwisterOfGuildJoinRequest());
        this._twisters.Add(0x51, new PacketTwisterOfGuildJoinResponse());
        this._twisters.Add(0x52, new PacketTwisterOfGuildList());
        this._twisters.Add(0x53, new PacketTwisterOfGuildKick());
        //// this.twisters.Add(0x54, new PacketTwisterOfGuildMasterResponse()); // implementation seems to be wrong
        this._twisters.Add(0x55, new PacketTwisterOfGuildMasterInfo());
        this._twisters.Add(0x57, new PacketTwister37());
        this._twisters.Add(0x61, new PacketTwisterOfGuildWarRequest());
        this._twisters.Add(0x66, new PacketTwisterOfGuildInfo());
        this._twisters.Add(0xB1, new PacketTwisterOfCastleSiege());
        this._twisters.Add(0xB2, new PacketTwister41());
        this._twisters.Add(0xB3, new PacketTwister42());
        this._twisters.Add(0xB7, new PacketTwisterOfCastleSiegeWeapon());
        this._twisters.Add(0xBD, new PacketTwisterOfCrywolf());
        this._twisters.Add(0xE2, new PacketTwisterOfGuildAssign());
        this._twisters.Add(0xE5, new PacketTwisterOfAllianceJoinRequest());
        this._twisters.Add(0x71, new PacketTwister47());
        this._twisters.Add(0x72, new PacketTwister48());
        this._twisters.Add(0x73, new PacketTwisterOfGameGuardAuth());
        this._twisters.Add(0x81, new PacketTwisterOfVaultMoney());
        this._twisters.Add(0x82, new PacketTwisterOfVaultClose());
        this._twisters.Add(0x83, new PacketTwisterOfVaultPassword());
        this._twisters.Add(0x86, new PacketTwisterOfChaosMachineMix());
        this._twisters.Add(0x87, new PacketTwisterOfChaosMachineClose());
        this._twisters.Add(0x90, new PacketTwisterOfDevilSquareEnter());
        this._twisters.Add(0x91, new PacketTwisterDevilSquareRemainingTime());
        this._twisters.Add(0x95, new PacketTwisterOfEventChipRegistration());
        this._twisters.Add(0x96, new PacketTwister58());
        this._twisters.Add(0x97, new PacketTwister59());
        this._twisters.Add(0xA0, new PacketTwisterOfQuestInfoRequest());
        this._twisters.Add(0xA2, new PacketTwisterOfQuestState());
        this._twisters.Add(0xA7, new PacketTwisterOfPetItemCommand());
        this._twisters.Add(0xA9, new PacketTwisterOfPetItemInfoRequest());
        this._twisters.Add(0x9A, new PacketTwisterOfBloodCastleEnterRequest());
        this._twisters.Add(0x9F, new PacketTwisterOfBloodCastleInfoRequest());
        this._twisters.Add(0x9D, new PacketTwisterOfLottoRegistration());
        this._twisters.Add(0xC0, new PacketTwisterOfFriendList());
        this._twisters.Add(0xC1, new PacketTwisterOfFriendAddRequest());
        this._twisters.Add(0xC2, new PacketTwisterOfFriendAddResponse());
        this._twisters.Add(0xC3, new PacketTwisterOfFriendDeleteRequest());
        this._twisters.Add(0xC4, new PacketTwisterOfFriendStateChange());
        this._twisters.Add(0xC5, new PacketTwisterOfLetterSendRequest());
        this._twisters.Add(0xC7, new PacketTwisterOfLetterReadRequest());
        this._twisters.Add(0xC8, new PacketTwisterOfLetterDeleteRequest());
        this._twisters.Add(0xC9, new PacketTwisterOfLetterListRequest());
        this._twisters.Add(0xCA, new PacketTwisterOfChatRoomCreateRequest());
        this._twisters.Add(0xCB, new PacketTwisterOfChatInvitationRequest());
        this._twisters.Add(0xF7, new PacketTwister78());
        this._twisters.Add(0xF6, new PacketTwister79());
        this._twisters.Add(0xD2, new PacketTwisterOfCashShop());
        this._twisters.Add(0xFA, new PacketTwister81());
    }

    /// <inheritdoc/>
    public void Encrypt(Span<byte> packet)
    {
        var packetType = packet.GetPacketType();
        var data = packet.Slice(packet.GetPacketHeaderSize() + 1);
        if (this._twisters.TryGetValue(packetType, out var twister))
        {
            twister.Twist(data);
        }
    }

    /// <inheritdoc/>
    public void Decrypt(Span<byte> packet)
    {
        var packetType = packet.GetPacketType();
        var data = packet.Slice(packet.GetPacketHeaderSize() + 1);
        if (this._twisters.TryGetValue(packetType, out var twister))
        {
            twister.Correct(data);
        }
    }
}