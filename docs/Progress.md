# Project feature progress

For the progress of the project as a whole, please have a look at the [projects page](https://github.com/MUnique/OpenMU/projects).

In this document we track the progress of the packet handlers.
Each packet handler requires some game logic behind it.

The complexity is a number between 1 (low complexity and effort) and 10 (high
complexity and effort). Complexity 0 means we wont implement it.

| Feature                     | Packet code       | Progress | Complexity | Note                                          |
|-----------------------------|-------------------|----------|------------|-----------------------------------------------|
| *Public chat*               | 0x00              | 100%     | 5          |                                               |
| *Whisper chat*              | 0x02              | 100%     | 5          |                                               |
| Client Checksum             | 0x03              | 0%       | 0          | Don't need - no security benefit              |
| Ping                        | 0x0E              | 0%       | 0          | Don't need - no security benefit              |
| *Animation*                   | 0x18              | 100%     | 2          |                                               |
| *SkillAttack*                | 0x19              | 100%     | 10         |                                               |
| MagicCancel                 | 0x1B              | 0%       | 2          |                                               |
| *WarpGate*                    | 0x1C              | 100%     | 5          |                                               |
| AreaSkill                   | 0x1E              | 100%      | 10         |                                               |
| *PickupItem*                  | 0x22              | 100%     | 6          |                                               |
| *DropItem*                    | 0x23              | 100%     | 6          |                                               |
| *InventoryMove*               | 0x24              | 100%     | 7          |                                               |
| *ConsumeItem*                 | 0x26              | 100%     | 5          |                                               |
| *TalkNPC*                     | 0x30              | 100%     | 5          |                                               |
| *CloseNPC*                    | 0x31              | 100%     | 1          |                                               |
| *BuyNPCItem*                  | 0x32              | 100%     | 5          |                                               |
| *SellNPCItem*                 | 0x33              | 100%     | 5          |                                               |
| *ItemRepair*                  | 0x34              | 100%     | 2          |                                               |
| *Trade Request*               | 0x36              | 100%     | 2          |                                               |
| *TradeResponse*               | 0x37              | 100%     | 2          |                                               |
| *TradeMoney*                  | 0x3A              | 100%     | 2          |                                               |
| *TradeOK*                     | 0x3C              | 100%     | 6          |                                               |
| *TradeCancel*                 | 0x3D              | 100%     | 3          |                                               |
| *PersonalShopGroup*           | 0x3F              | 100%     | 10         |                                               |
| *PartyRequest*                | 0x40              | 100%     | 2          |                                               |
| *PartyRequestAnswer*          | 0x41              | 100%     | 4          |                                               |
| *RequestPartyList*            | 0x42              | 100%     | 5          |                                               |
| *PartyKick*                   | 0x43              | 100%     | 3          |                                               |
| *GuildJoinRequest*            | 0x50              | 100%     | 1          |                                               |
| *GuildJoinAnswer*             | 0x51              | 100%     | 1          |                                               |
| *RequestGuildList*            | 0x52              | 100%     | 3          |                                               |
| *GuildKickPlayer*             | 0x53              | 100%     | 1          |                                               |
| *GuildMasterAnswer*           | 0x54              | 100%     | 1          |                                               |
| *GuildMasterInfoSave*         | 0x55              | 100%     | 2          |                                               |
| GuildMasterCreateCancel     | 0x56              | 100%       | 1          |                                               |
| GuildWarReqRes              | 0x61              | 100%       | 3          |                                               |
| *GuildInfoRequest*            | 0x66              | 100%     | 2          |                                               |
| GGAuth                      | 0x73              | 0%       | 0          |                                               |
| *WarehouseMoneyInOut*         | 0x81              | 100%     | 3          |                                               |
| *WarehouseClose*              | 0x82              | 100%     | 3          |                                               |
| WarehousePassword           | 0x83              | 100%       | 1          |                                               |
| ChaosMachineMix             | 0x86              | 100%     | 5          |                                               |
| *ChaosMachineClose*           | 0x87              | 100%     | 1          |                                               |
| *WarpCommand*                 | 0x8E              | 100%     | 5          |                                               |
| DevilSquareEnter            | 0x90              | 100%       | 2          |                                               |
| DevilSquareRemainingTime    | 0x91              | 100%       | 1          |                                               |
| RegEventChip                | 0x95              | 0%       | 1          |                                               |
| GetMutoNum                  | 0x96              | 0%       | 1          |                                               |
| UseEndEventChip             | 0x97              | 0%       | 1          |                                               |
| UseRenaExchangeZen          | 0x98              | 0%       | 1          |                                               |
| RequestChangeServer         | 0x99              | 0%       | 0          |                                               |
| RequestEnterBC              | 0x9A              | 100%     | 3          |                                               |
| BCStuff                     | 0x9B              | 100%     | 1          |                                               |
| RequestLottoRegister        | 0x9D              | 0%       | 0          |                                               |
| RequestEventEnterCount      | 0x9F              | 0%       | 0          |                                               |
| *RequestQuestInfo*            | 0xA0              | 100%     | 4          |                                               |
| *SetQuestState*               | 0xA2              | 100%     | 2          |                                               |
| PetItemCommand              | 0xA7              | 100%     | 5          |                                               |
| RequestPetItemInfo          | 0xA9              | 100%     | 3          |                                               |
| RequestDuelStart            | 0xAA              | 0%       | 4          |                                               |
| RequestDuelEnd              | 0xAB              | 0%       | 2          |                                               |
| RequestDuelOK               | 0xAC              | 0%       | 2          |                                               |
| RequestEnterChaosCastle     | 0xAF              | 100%     | 4          |                                               |
| TargetTeleport              | 0xB0              | 0%       | 3          |                                               |
| ChangeServerAuth            | 0xB1              | 0%       | 0          |                                               |
| CastleSiegeGroup            | 0xB1              | 0%       | 10         |                                               |
| CSRequest_NPC_DB_List       | 0xB3              | 0%       | 3          |                                               |
| RequestCSRegGuildList       | 0xB4              | 0%       | 2          |                                               |
| RequestCSAttGuildList       | 0xB5              | 0%       | 2          |                                               |
| RequestCSWeaponUse          | 0xB7              | 0%       | 1          |                                               |
| *LoggedIn*                    | 0xB8              | 100%     | 1          |                                               |
| RequestGuildMarkCastleOwner | 0xB9              | 0%       | 1          |                                               |
| *JewelMix*                    | 0xBC              | 100%     | 4          |                                               |
| CrywolfGroup                | 0xBD              | 0%       | 10         |                                               |
| GuildAssignStatus           | 0xBE              | 0%       | 1          |                                               |
| FriendListRequest           | 0xC0              | 0%       | 0          | Not needed, friend list is sent automatically |
| *FriendAdd*                   | 0xC1              | 100%     | 2          |                                               |
| *WaitFriendAdd*               | 0xC2              | 100%     | 2          |                                               |
| *FriendDelete*                | 0xC3              | 100%     | 1          |                                               |
| *FriendStateClient*           | 0xC4              | 100%     | 1          |                                               |
| *FriendMemoSend*              | 0xC5              | 100%     | 2          |                                               |
| *FriendMemoReadRequest*       | 0xC7              | 100%     | 2          |                                               |
| *FriendMemoDelete*            | 0xC8              | 100%     | 1          |                                               |
| FriendMemoListRequest       | 0xC9              | 0%       | 0          | Not needed, letter list is sent automatically |
| *ChatRoomCreate*              | 0xCA              | 100%     | 2          |                                               |
| *ChatRoomInvitationReq*       | 0xCB              | 100%     | 2          |                                               |
| KanturuGroup                | 0xD1              | 0%       | 10         |                                               |
| *AreaSkillHit*                | 0xDB              | 100%     | 10         |                                               |
| GuildAssignType             | 0xE2              | 0%       | 2          |                                               |
| RequestAllyJoinLeave        | 0xE5              | 0%       | 3          |                                               |
| AnswerAllyJoinLeave         | 0xE6              | 0%       | 3          |                                               |
| RequestAllyList             | 0xE9              | 0%       | 3          |                                               |
| RequestAllyKickGuild        | 0xEB              | 0%       | 3          |                                               |
| *LoginLogoutGroup*            | 0xF1              | 100%     | 10         |                                               |
| *CharacterGroup*              | 0xF3              | 100%     | 10         |                                               |
|   - *Character list* | 0xF300            |      |          |                                               |
|   - *Character creation* | 0xF301            |      |          |                                               |
|   - *Character delete request* | 0xF302            |      |          |                                               |
|   - *Character select request* | 0xF303            |      |          |                                               |
|   - *Character stat increase request* | 0xF306            |      |          |                                               |
|   - *Character focus request* | 0xF315            |      |          |                                               |
|   - *Save key configuration* | 0xF330            |      |          |                                               |
|   - *Master skill level increase request* | 0xF352            |      |          |                                               
| CashShopGroup               | 0xF5              | 0%       | 10         | Low priority                                  |
| *Hit*                         | 0x11      | 100%     | 10         |                                               |
| *Teleport*                    | 0x15 | 100%     | 2          |                                               |
| *Walk*                        | 0xD4     | 100%     | 10         |                                               |

