# C1 F6 0D - QuestCompletionResponse (server gửi)

## Được gửi khi nào

Khi server xác nhận quest đã được hoàn thành.

## Hành động phía client

Client hiển thị thành công và có thể yêu cầu danh sách quest khả dụng tiếp theo.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 9 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xF6 | Packet header - packet type identifier |
| 3 | 1 | Byte | 0x0D | Packet header - sub packet type identifier |
| 4 | 2 | ShortLittleEndian |  | QuestNumber |
| 6 | 2 | ShortLittleEndian |  | QuestGroup |
| 8 | 1 | Boolean |  | IsQuestCompleted |
