# Tài liệu Packet

Trong thư mục này là tài liệu về các packet message được trao đổi giữa client
và server.

Theo mô tả trong project readme, protocol chính là bản ENG (English) của
Season 6 Episode 3.

Mỗi packet có một file riêng. Tài liệu packet được sinh tự động bằng XSLT.
Nguồn XML nằm trong
[MUnique.OpenMU.Network.Packets](https://github.com/MUnique/OpenMU/tree/master/src/Network/Packets).

Nếu bạn muốn đóng góp tài liệu packet, hãy cập nhật các file nguồn này.
Nếu muốn build lại markdown, bạn cần cài `NodeJS 16+` và rebuild project
`MUnique.OpenMU.Network.Packets`.

Toàn bộ các file trong `docs/Packets` hiện đều có bản tiếng Việt trong thư mục
này (kể cả `PacketTypes.md`, `ServerToClient.md`, v.v.). Các packet ngoài các
đợt liệt kê bên dưới chủ yếu được sinh bằng script
[`scripts/translate_missing_packet_docs.py`](../../../scripts/translate_missing_packet_docs.py);
khi chỉnh sửa protocol, nên rà lại thuật ngữ trong bản dịch tự động.

## Loại packet

Packet MU Online có thể bắt đầu bằng 4 giá trị byte (0xC1 đến 0xC4), mỗi loại
có ý nghĩa khác nhau. Xem mô tả tại [PacketTypes](../Packets/PacketTypes.md).

## Danh mục packet

- [Từ Game Server tới Client](../Packets/ServerToClient.md)
- [Từ Client tới Game Server](../Packets/ClientToServer.md)
- [Giữa Connect Server và Client](ConnectServer.md)
- [Giữa Chat Server và Client](../Packets/ChatServer.md)

## Các file đã dịch trong đợt 1

- [C1 F4 06 - ServerListRequest](C1-F4-06-ServerListRequest_by-client.md)
- [C2 F4 06 - ServerListResponse](C2-F4-06-ServerListResponse_by-server.md)
- [C1 F4 03 - ConnectionInfoRequest](C1-F4-03-ConnectionInfoRequest_by-client.md)
- [C1 F4 03 - ConnectionInfo](C1-F4-03-ConnectionInfo_by-server.md)
- [C1 00 - Authenticate](C1-00-Authenticate_by-client.md)
- [C1 F1 01 - LoginResponse](C1-F1-01-LoginResponse_by-server.md)
- [C1 F3 00 - CharacterList](C1-F3-00-CharacterList_by-server.md)
- [C1 F3 03 - SelectCharacter](C1-F3-03-SelectCharacter_by-client.md)
- [C3 F3 03 - CharacterInformation](C3-F3-03-CharacterInformation_by-server.md)
- [C1 D4 - WalkRequest](C1-D4-WalkRequest_by-client.md)
- [C1 D4 - ObjectWalked](C1-D4-ObjectWalked_by-server.md)

## Các file đã dịch trong đợt 2

- [C1 11 - HitRequest](C1-11-HitRequest_by-client.md)
- [C1 11 - ObjectHit](C1-11-ObjectHit_by-server.md)
- [C1 17 - ObjectGotKilled](C1-17-ObjectGotKilled_by-server.md)
- [C1 22 - PickupItemRequest075](C1-22-PickupItemRequest075_by-client.md)
- [C3 22 - ItemAddedToInventory](C3-22-ItemAddedToInventory_by-server.md)
- [C3 23 - DropItemRequest](C3-23-DropItemRequest_by-client.md)
- [C1 23 - ItemDropResponse](C1-23-ItemDropResponse_by-server.md)
- [C3 24 - ItemMoveRequest](C3-24-ItemMoveRequest_by-client.md)
- [C3 24 - ItemMoved](C3-24-ItemMoved_by-server.md)
- [C1 3D - TradeCancel](C1-3D-TradeCancel_by-client.md)
- [C1 3D - TradeFinished](C1-3D-TradeFinished_by-server.md)

## Các file đã dịch trong đợt 3 (giai đoạn đầu)

- [C1 40 - PartyRequest](C1-40-PartyRequest_by-server.md)
- [C1 42 01 - PartyList](C1-42-01-PartyList_by-server.md)
- [C1 43 - RemovePartyMember](C1-43-RemovePartyMember_by-server.md)
- [C1 50 - GuildJoinRequest](C1-50-GuildJoinRequest_by-client.md)
- [C1 51 - GuildJoinResponse](C1-51-GuildJoinResponse_by-server.md)
- [C2 52 - GuildList](C2-52-GuildList_by-server.md)
- [C1 90 - DevilSquareEnterRequest](C1-90-DevilSquareEnterRequest_by-client.md)
- [C1 90 - DevilSquareEnterResult](C1-90-DevilSquareEnterResult_by-server.md)
- [C1 41 - PartyInviteResponse](C1-41-PartyInviteResponse_by-client.md)
- [C1 44 - PartyHealthUpdate](C1-44-PartyHealthUpdate_by-server.md)
- [C1 53 - GuildKickPlayerRequest](C1-53-GuildKickPlayerRequest_by-client.md)
- [C1 53 - GuildKickResponse](C1-53-GuildKickResponse_by-server.md)
- [C1 54 - ShowGuildMasterDialog](C1-54-ShowGuildMasterDialog_by-server.md)
- [C1 56 - GuildCreationResult](C1-56-GuildCreationResult_by-server.md)

## Các file đã dịch trong đợt 4 (Messenger/Duel/IllusionTemple)

- [C1 C1 - FriendAddRequest](C1-C1-FriendAddRequest_by-client.md)
- [C1 C1 01 - FriendAdded](C1-C1-01-FriendAdded_by-server.md)
- [C1 C3 - FriendDelete](C1-C3-FriendDelete_by-client.md)
- [C1 C3 01 - FriendDeleted](C1-C3-01-FriendDeleted_by-server.md)
- [C1 AA 01 - DuelStartResult](C1-AA-01-DuelStartResult_by-server.md)
- [C1 AA 03 - DuelEnd](C1-AA-03-DuelEnd_by-server.md)
- [C1 BF 00 - IllusionTempleEnterRequest](C1-BF-00-IllusionTempleEnterRequest_by-client.md)
- [C1 BF 00 - IllusionTempleEnterResult](C1-BF-00-IllusionTempleEnterResult_by-server.md)

## Các file đã dịch trong đợt 5 (Quest/CashShop)

- [C1 F6 0A - AvailableQuests](C1-F6-0A-AvailableQuests_by-server.md)
- [C1 F6 0B - QuestProceedRequest](C1-F6-0B-QuestProceedRequest_by-client.md)
- [C1 F6 03 - QuestEventResponse](C1-F6-03-QuestEventResponse_by-server.md)
- [C1 F6 0D - QuestCompletionRequest](C1-F6-0D-QuestCompletionRequest_by-client.md)
- [C1 F6 0D - QuestCompletionResponse](C1-F6-0D-QuestCompletionResponse_by-server.md)
- [C1 D2 03 - CashShopItemBuyRequest](C1-D2-03-CashShopItemBuyRequest_by-client.md)
- [C1 D2 04 - CashShopItemGiftRequest](C1-D2-04-CashShopItemGiftRequest_by-client.md)
- [C1 D2 0B - CashShopStorageItemConsumeRequest](C1-D2-0B-CashShopStorageItemConsumeRequest_by-client.md)

## Các file đã dịch trong đợt 6 (Login/Character mở rộng)

- [C1 F1 00 - GameServerEntered](C1-F1-00-GameServerEntered_by-server.md)
- [C3 F1 01 - LoginLongPassword](C3-F1-01-LoginLongPassword_by-client.md)
- [C3 F1 02 - LogoutResponse](C3-F1-02-LogoutResponse_by-server.md)
- [C1 F3 01 - CreateCharacter](C1-F3-01-CreateCharacter_by-client.md)
- [C1 F3 01 - CharacterCreationSuccessful](C1-F3-01-CharacterCreationSuccessful_by-server.md)
- [C1 F3 01 - CharacterCreationFailed](C1-F3-01-CharacterCreationFailed_by-server.md)
- [C1 F3 02 - DeleteCharacter](C1-F3-02-DeleteCharacter_by-client.md)
- [C1 F3 02 - CharacterDeleteResponse](C1-F3-02-CharacterDeleteResponse_by-server.md)
- [C1 F3 06 - IncreaseCharacterStatPoint](C1-F3-06-IncreaseCharacterStatPoint_by-client.md)
- [C1 F3 06 - CharacterStatIncreaseResponse](C1-F3-06-CharacterStatIncreaseResponse_by-server.md)

## Các file đã dịch trong đợt 7 (075 / 095)

- [C3 F1 01 - Login075](C3-F1-01-Login075_by-client.md)
- [C1 F4 03 - ConnectionInfoRequest075](C1-F4-03-ConnectionInfoRequest075_by-client.md)
- [C1 F3 00 - CharacterList075](C1-F3-00-CharacterList075_by-server.md)
- [C3 F3 03 - CharacterInformation075](C3-F3-03-CharacterInformation075_by-server.md)
- [C1 10 - WalkRequest075](C1-10-WalkRequest075_by-client.md)
- [C1 10 - ObjectWalked075](C1-10-ObjectWalked075_by-server.md)
- [C1 F3 00 - CharacterList095](C1-F3-00-CharacterList095_by-server.md)
- [C3 19 - TargetedSkill095](C3-19-TargetedSkill095_by-client.md)

## Các file đã dịch trong đợt 8 (Combat/Skill — 075 / 095)

- [C1 19 - TargetedSkill075](C1-19-TargetedSkill075_by-client.md)
- [C1 19 - SkillAnimation075](C1-19-SkillAnimation075_by-server.md)
- [C1 1D - AreaSkillHit075](C1-1D-AreaSkillHit075_by-client.md)
- [C1 1E - AreaSkill075](C1-1E-AreaSkill075_by-client.md)
- [C1 1E - AreaSkillAnimation075](C1-1E-AreaSkillAnimation075_by-server.md)
- [C1 1B - MagicEffectCancelled075](C1-1B-MagicEffectCancelled075_by-server.md)
- [C3 19 - SkillAnimation095](C3-19-SkillAnimation095_by-server.md)
- [C3 1E - AreaSkill095](C3-1E-AreaSkill095_by-client.md)

## Các file đã dịch trong đợt 9 (095 — skill / hồi sinh / scope)

- [C3 1D - AreaSkillHit095](C3-1D-AreaSkillHit095_by-client.md)
- [C3 1E - AreaSkillAnimation095](C3-1E-AreaSkillAnimation095_by-server.md)
- [C1 F3 04 - RespawnAfterDeath095](C1-F3-04-RespawnAfterDeath095_by-server.md)
- [C1 F3 11 - SkillAdded095](C1-F3-11-SkillAdded095_by-server.md)
- [C1 F3 11 - SkillRemoved095](C1-F3-11-SkillRemoved095_by-server.md)
- [C2 12 - AddCharactersToScope095](C2-12-AddCharactersToScope095_by-server.md)
- [C2 13 - AddNpcsToScope095](C2-13-AddNpcsToScope095_by-server.md)
- [C2 1F - AddSummonedMonstersToScope095](C2-1F-AddSummonedMonstersToScope095_by-server.md)

## Các file đã dịch trong đợt 10 (075 — scope / map / skill list)

- [C2 12 - AddCharactersToScope075](C2-12-AddCharactersToScope075_by-server.md)
- [C2 13 - AddNpcsToScope075](C2-13-AddNpcsToScope075_by-server.md)
- [C2 1F - AddSummonedMonstersToScope075](C2-1F-AddSummonedMonstersToScope075_by-server.md)
- [C2 45 - AddTransformedCharactersToScope075](C2-45-AddTransformedCharactersToScope075_by-server.md)
- [C3 1C - MapChanged075](C3-1C-MapChanged075_by-server.md)
- [C3 1C - EnterGateRequest075](C3-1C-EnterGateRequest075_by-client.md)
- [C1 F3 04 - RespawnAfterDeath075](C1-F3-04-RespawnAfterDeath075_by-server.md)
- [C1 F3 11 - SkillListUpdate075](C1-F3-11-SkillListUpdate075_by-server.md)

## Các file đã dịch trong đợt 11 (075 — party/guild/item/skill)

- [C1 F3 11 - SkillAdded075](C1-F3-11-SkillAdded075_by-server.md)
- [C1 F3 11 - SkillRemoved075](C1-F3-11-SkillRemoved075_by-server.md)
- [C1 26 - ConsumeItemRequest075](C1-26-ConsumeItemRequest075_by-client.md)
- [C2 20 - MoneyDropped075](C2-20-MoneyDropped075_by-server.md)
- [C1 42 01 - PartyList075](C1-42-01-PartyList075_by-server.md)
- [C2 52 - GuildList075](C2-52-GuildList075_by-server.md)
- [C2 5A - GuildInformations075](C2-5A-GuildInformations075_by-server.md)
- [C1 5C - SingleGuildInformation075](C1-5C-SingleGuildInformation075_by-server.md)

## Appearance

Một số packet có trường gọi là "Appearance". Định dạng nhị phân xem tại
[Appearance](../Packets/Appearance.md).
