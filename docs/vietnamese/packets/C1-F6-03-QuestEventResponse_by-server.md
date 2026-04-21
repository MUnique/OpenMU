# C1 F6 03 - QuestEventResponse (server gửi)

## Được gửi khi nào

Sau khi game client yêu cầu danh sách event quest khi vào game.
Có vẻ packet này chỉ gửi khi nhân vật không thuộc Gen.

## Hành động phía client

Chưa rõ.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 12 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xF6 | Packet header - packet type identifier |
| 3 | 1 | Byte | 0x03 | Packet header - sub packet type identifier |
| 4 | QuestIdentification.Length *  | Array of QuestIdentification |  | Quests |

### Cấu trúc QuestIdentification

Thông tin định danh một quest.

Độ dài: 4 bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortLittleEndian |  | Number |
| 2 | 2 | ShortLittleEndian |  | Group |
