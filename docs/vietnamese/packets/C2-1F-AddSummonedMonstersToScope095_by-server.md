# C2 1F - AddSummonedMonstersToScope095 (server gửi)

## Được gửi khi nào

Một hoặc nhiều quái triệu hồi đi vào phạm vi quan sát của người chơi.

## Hành động phía client

Client thêm các quái đó vào bản đồ đang hiển thị.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC2 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 2 | Short |  | Packet header - độ dài packet |
| 3 | 1 | Byte | 0x1F | Packet header - packet type identifier |
| 4 | 1 | Byte |  | MonsterCount |
| 5 | SummonedMonsterData.Length * MonsterCount | Array of SummonedMonsterData |  | SummonedMonsters |

### Cấu trúc SummonedMonsterData

Chứa dữ liệu của một quái được triệu hồi.

Độ dài: 20 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortBigEndian |  | Id |
| 2 | 1 | Byte |  | TypeNumber |
| 4 << 0 | 1 bit | Boolean |  | IsPoisoned |
| 4 << 1 | 1 bit | Boolean |  | IsIced |
| 4 << 2 | 1 bit | Boolean |  | IsDamageBuffed |
| 4 << 3 | 1 bit | Boolean |  | IsDefenseBuffed |
| 5 | 1 | Byte |  | CurrentPositionX |
| 6 | 1 | Byte |  | CurrentPositionY |
| 7 | 1 | Byte |  | TargetPositionX |
| 8 | 1 | Byte |  | TargetPositionY |
| 9 | 4 bit | Byte |  | Rotation |
| 10 | 10 | String |  | OwnerCharacterName |
