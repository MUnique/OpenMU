# C2 13 - AddNpcsToScope095 (server gửi)

## Được gửi khi nào

Một hoặc nhiều NPC đi vào phạm vi quan sát của người chơi.

## Hành động phía client

Client thêm các NPC vào bản đồ đang hiển thị.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC2 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 2 | Short |  | Packet header - độ dài packet |
| 3 | 1 | Byte | 0x13 | Packet header - packet type identifier |
| 4 | 1 | Byte |  | NpcCount |
| 5 | NpcData.Length * NpcCount | Array of NpcData |  | NPCs |

### Cấu trúc NpcData

Chứa dữ liệu của một NPC.

Độ dài: 12 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortBigEndian |  | Id |
| 2 | 1 | Byte |  | TypeNumber |
| 4 << 0 | 1 bit | Boolean |  | IsPoisoned |
| 4 << 1 | 1 bit | Boolean |  | IsIced |
| 4 << 2 | 1 bit | Boolean |  | IsDamageBuffed |
| 4 << 3 | 1 bit | Boolean |  | IsDefenseBuffed |
| 6 | 1 | Byte |  | CurrentPositionX |
| 7 | 1 | Byte |  | CurrentPositionY |
| 8 | 1 | Byte |  | TargetPositionX |
| 9 | 1 | Byte |  | TargetPositionY |
| 10 | 4 bit | Byte |  | Rotation |
