// <copyright file="PacketType.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer
{
    /// <summary>
    /// The type of the packet. The value is contained in the packet header.
    /// </summary>
    public enum PacketType : byte
    {
        /// <summary>
        /// A speak packet.
        /// </summary>
        Speak = 0, ////+

        /// <summary>
        /// A Whisper packet.
        /// </summary>
        Whisper = 2, ////+

        /// <summary>
        /// A walk packet.
        /// </summary>
        /// <remarks>ENG protocol specific.</remarks>
        Walk = 0xD4,

        /// <summary>
        /// A teleport packet.
        /// </summary>
        /// <remarks>ENG protocol specific.</remarks>
        Teleport = 0x15,

        /// <summary>
        /// A hit packet.
        /// </summary>
        /// <remarks>ENG protocol specific.</remarks>
        Hit = 0x11,

        /// <summary>
        /// A LoginLogoutGroup packet.
        /// </summary>
        LoginLogoutGroup = 0xF1, ////+

        /// <summary>
        /// A Ping packet.
        /// </summary>
        Ping = 0x0E, ////+

        /// <summary>
        /// A Checksum packet.
        /// </summary>
        Checksum = 0x03,

        /// <summary>
        /// A SkillAttack packet.
        /// </summary>
        SkillAttack = 0x19, ////+

        /// <summary>
        /// A WarpGate packet.
        /// </summary>
        WarpGate = 0x1C, ////+

        /// <summary>
        /// A TargetTeleport packet.
        /// </summary>
        TargetTeleport = 0xB0,

        /// <summary>
        /// A AreaSkill packet.
        /// </summary>
        AreaSkill = 0x1E, ////+

        /// <summary>
        /// A AreaSkillHit packet.
        /// </summary>
        AreaSkillHit = 0xDB, ////+

        /// <summary>
        /// A PickupItem packet.
        /// </summary>
        PickupItem = 0x22, // +

        /// <summary>
        /// A DropItem packet.
        /// </summary>
        DropItem = 0x23, // +

        /// <summary>
        /// A InventoryMove packet.
        /// </summary>
        InventoryMove = 0x24, // +

        /// <summary>
        /// A ConsumeItem packet.
        /// </summary>
        ConsumeItem = 0x26, ////+

        /// <summary>
        /// A TalkNPC packet.
        /// </summary>
        TalkNPC = 0x30, ////+

        /// <summary>
        /// A BuyNPCItem packet.
        /// </summary>
        BuyNPCItem = 0x32, ////+

        /// <summary>
        /// A SellNPCItem packet.
        /// </summary>
        SellNPCItem = 0x33, ////+

        /// <summary>
        /// A ItemRepair packet.
        /// </summary>
        ItemRepair = 0x34, ////+

        /// <summary>
        /// A TradeRequest packet.
        /// </summary>
        TradeRequest = 0x36, ////+

        /// <summary>
        /// A TradeButton packet.
        /// </summary>
        TradeButton = 0x3C, ////+

        /// <summary>
        /// A TradeCancel packet.
        /// </summary>
        TradeCancel = 0x3D, ////+

        /// <summary>
        /// A PersonalShopGroup packet.
        /// </summary>
        PersonalShopGroup = 0x3F, ////+

        /// <summary>
        /// A PartyRequest packet.
        /// </summary>
        PartyRequest = 0x40, ////+

        /// <summary>
        /// A PartyRequestAnswer packet.
        /// </summary>
        PartyRequestAnswer = 0x41, ////+

        /// <summary>
        /// A ChangeServerAuth packet.
        /// </summary>
        ChangeServerAuth = 0xB1, ////dont need yet--

        /// <summary>
        /// A GGAuth packet.
        /// </summary>
        GGAuth = 0x73, ////dont need yet--

        /// <summary>
        /// A LoggedIn packet.
        /// </summary>
        LoggedIn = 0xB8, ////+

        /// <summary>
        /// A CharacterGroup packet.
        /// </summary>
        CharacterGroup = 0xF3, ////+

        /// <summary>
        /// A Animation packet.
        /// </summary>
        Animation = 0x18, ////+

        /// <summary>
        /// A MagicCancel packet.
        /// </summary>
        MagicCancel = 0x1B,

        /// <summary>
        /// A CloseNPC packet.
        /// </summary>
        CloseNPC = 0x31, ////+

        /// <summary>
        /// A TradeAccept packet.
        /// </summary>
        TradeAccept = 0x37, ////+

        /// <summary>
        /// A TradeMoney packet.
        /// </summary>
        TradeMoney = 0x3A, ////+

        /// <summary>
        /// A RequestPartyList packet.
        /// </summary>
        RequestPartyList = 0x42, ////+

        /// <summary>
        /// A PartyKick packet.
        /// </summary>
        PartyKick = 0x43, ////+

        ////Guild Stuff:

        /// <summary>
        /// A GuildJoinRequest packet.
        /// </summary>
        GuildJoinRequest = 0x50, ////+

        /// <summary>
        /// A GuildJoinAnswer packet.
        /// </summary>
        GuildJoinAnswer = 0x51, ////+

        /// <summary>
        /// A RequestGuildList packet.
        /// </summary>
        RequestGuildList = 0x52, ////+

        /// <summary>
        /// A GuildKickPlayer packet.
        /// </summary>
        GuildKickPlayer = 0x53, ////+

        /// <summary>
        /// A GuildMasterAnswer packet.
        /// </summary>
        GuildMasterAnswer = 0x54, ////+

        /// <summary>
        /// A GuildMasterInfoSave packet.
        /// </summary>
        GuildMasterInfoSave = 0x55, ////+

        /// <summary>
        /// A GuildMasterCreateCancel packet.
        /// </summary>
        GuildMasterCreateCancel = 0x56, ////dun need to handle i think

        /// <summary>
        /// A GuildWarReqRes packet.
        /// </summary>
        GuildWarReqRes = 0x61,

        /// <summary>
        /// A GuildInfoRequest packet.
        /// </summary>
        GuildInfoRequest = 0x66, ////+

        ////Castle Siege:

        /// <summary>
        /// A CastleSiegeGroup packet.
        /// </summary>
        CastleSiegeGroup = 0xB1,

        /// <summary>
        /// A CSRequest_NPC_DB_List packet.
        /// </summary>
        CSRequest_NPC_DB_List = 0xB3,

        /// <summary>
        /// A RequestCSRegGuildList packet.
        /// </summary>
        RequestCSRegGuildList = 0xB4,

        /// <summary>
        /// A RequestCSAttGuildList packet.
        /// </summary>
        RequestCSAttGuildList = 0xB5,

        /// <summary>
        /// A RequestCSWeaponUse packet.
        /// </summary>
        RequestCSWeaponUse = 0xB7,

        /// <summary>
        /// A RequestGuildMarkCastleOwner packet.
        /// </summary>
        RequestGuildMarkCastleOwner = 0xB9,
        ////Lahap:

        /// <summary>
        /// A JewelMix packet.
        /// </summary>
        JewelMix = 0xBC, ////+

        /// <summary>
        /// A CrywolfGroup packet.
        /// </summary>
        CrywolfGroup = 0xBD,

        ////Alliance Stuff:

        /// <summary>
        /// A GuildAssignStatus packet.
        /// </summary>
        GuildAssignStatus = 0xBE,

        /// <summary>
        /// A GuildAssignType packet.
        /// </summary>
        GuildAssignType = 0xE2,

        /// <summary>
        /// A RequestAllyJoinLeave packet.
        /// </summary>
        RequestAllyJoinLeave = 0xE5,

        /// <summary>
        /// A AnswerAllyJoinLeave packet.
        /// </summary>
        AnswerAllyJoinLeave = 0xE6,

        /// <summary>
        /// A RequestAllyList packet.
        /// </summary>
        RequestAllyList = 0xE9,

        /// <summary>
        /// A RequestAllyKickGuild packet.
        /// </summary>
        RequestAllyKickGuild = 0xEB,

        ////Vault:

        /// <summary>
        /// A VaultMoneyInOut packet.
        /// </summary>
        VaultMoneyInOut = 0x81, ////+

        /// <summary>
        /// A VaultClose packet.
        /// </summary>
        VaultClose = 0x82, ////+

        /// <summary>
        /// A VaultPassword packet.
        /// </summary>
        VaultPassword = 0x83, ////+

        ////CM:

        /// <summary>
        /// A ChaosMachineMix packet.
        /// </summary>
        ChaosMachineMix = 0x86, ////+

        /// <summary>
        /// A ChaosMachineClose packet.
        /// </summary>
        ChaosMachineClose = 0x87, ////+

        /// <summary>
        /// A WarpCommand packet.
        /// </summary>
        WarpCommand = 0x8E, ////+

        /// <summary>
        /// A DevilSquareEnter packet.
        /// </summary>
        DevilSquareEnter = 0x90,

        /// <summary>
        /// A DevilSquareRemainingTime packet.
        /// </summary>
        DevilSquareRemainingTime = 0x91,

        /// <summary>
        /// A RegEventChip packet.
        /// </summary>
        RegEventChip = 0x95,

        /// <summary>
        /// A GetMutoNum packet.
        /// </summary>
        GetMutoNum = 0x96, ////?

        /// <summary>
        /// A UseEndEventChip packet.
        /// </summary>
        UseEndEventChip = 0x97, ////?

        /// <summary>
        /// A UseRenaExchangeZen packet.
        /// </summary>
        UseRenaExchangeZen = 0x98, ////?

        /// <summary>
        /// A RequestChangeServer packet.
        /// </summary>
        RequestChangeServer = 0x99,

        /// <summary>
        /// A RequestQuestInfo packet.
        /// </summary>
        RequestQuestInfo = 0xA0,

        /// <summary>
        /// A SetQuestState packet.
        /// </summary>
        SetQuestState = 0xA2,

        /// <summary>
        /// A PetItemCommand packet.
        /// </summary>
        PetItemCommand = 0xA7,

        /// <summary>
        /// A RequestPetItemInfo packet.
        /// </summary>
        RequestPetItemInfo = 0xA9, ////(Horse and Raven Exp, lvl etc...)

        ////Duel:

        /// <summary>
        /// A RequestDuelStart packet.
        /// </summary>
        RequestDuelStart = 0xAA,

        /// <summary>
        /// A RequestDuelEnd packet.
        /// </summary>
        RequestDuelEnd = 0xAB,

        /// <summary>
        /// A RequestDuelOK packet.
        /// </summary>
        RequestDuelOK = 0xAC,
        ////BC:

        /// <summary>
        /// A RequestEnterBC packet.
        /// </summary>
        RequestEnterBC = 0x9A,

        /// <summary>
        /// A BCStuff packet.
        /// </summary>
        BCStuff = 0x9B,

        /// <summary>
        /// A RequestEventEnterCount packet.
        /// </summary>
        RequestEventEnterCount = 0x9F,

        /// <summary>
        /// A RequestLottoRegister packet.
        /// </summary>
        RequestLottoRegister = 0x9D,

        /// <summary>
        /// A RequestEnterChaosCastle packet.
        /// </summary>
        RequestEnterChaosCastle = 0xAF,
        ////MUssenger / Friendlist:

        /// <summary>
        /// A FriendListRequest packet.
        /// </summary>
        FriendListRequest = 0xC0, ////? i think it doesnt exit. friendlist is always sent after char selection

        /// <summary>
        /// A FriendAdd packet.
        /// </summary>
        FriendAdd = 0xC1, ////+

        /// <summary>
        /// A FriendAddReponse packet.
        /// </summary>
        FriendAddReponse = 0xC2, ////+

        /// <summary>
        /// A FriendDelete packet.
        /// </summary>
        FriendDelete = 0xC3, ////+

        /// <summary>
        /// A FriendStateClient packet.
        /// </summary>
        FriendStateClient = 0xC4, ////+

        /// <summary>
        /// A FriendMemoSend packet.
        /// </summary>
        FriendMemoSend = 0xC5, ////+

        /// <summary>
        /// A FriendMemoReadRequest packet.
        /// </summary>
        FriendMemoReadRequest = 0xC7, ////+

        /// <summary>
        /// A FriendMemoDelete packet.
        /// </summary>
        FriendMemoDelete = 0xC8, ////+

        /// <summary>
        /// A FriendMemoListRequest packet.
        /// </summary>
        FriendMemoListRequest = 0xC9, ////? did not see this yet

        /// <summary>
        /// A ChatRoomCreate packet.
        /// </summary>
        ChatRoomCreate = 0xCA, ////+

        /// <summary>
        /// A ChatRoomInvitationReq packet.
        /// </summary>
        ChatRoomInvitationReq = 0xCB,

        /// <summary>
        /// A KanturuGroup packet.
        /// </summary>
        KanturuGroup = 0xD1,

        /// <summary>
        /// A CashShopGroup packet.
        /// </summary>
        CashShopGroup = 0xD2,
    }
}
