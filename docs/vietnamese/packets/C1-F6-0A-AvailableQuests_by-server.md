# C1 F6 0A - AvailableQuests (server gửi)

## Được gửi khi nào

Sau khi game client yêu cầu danh sách quest khả dụng qua hội thoại NPC.

## Hành động phía client

Client hiển thị các quest khả dụng cho NPC đang tương tác.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte |  | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xF6 | Packet header - packet type identifier |
| 3 | 1 | Byte | 0x0A | Packet header - sub packet type identifier |
| 4 | 2 | ShortLittleEndian |  | QuestNpcNumber |
| 6 | 2 | ShortLittleEndian |  | QuestCount |
| 8 | QuestIdentification.Length * QuestCount | Array of QuestIdentification |  | Quests |

### Cấu trúc QuestIdentification

Thông tin định danh một quest.

Độ dài: 4 bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortLittleEndian |  | Number |
| 2 | 2 | ShortLittleEndian |  | Group |
