# C1 F6 0D - QuestCompletionRequest (client gửi)

## Được gửi khi nào

Khi game client yêu cầu hoàn thành một quest đang hoạt động.

## Hành động phía server

Server kiểm tra điều kiện hoàn thành quest.

- Nếu không đạt điều kiện: không có thay đổi.
- Nếu đạt điều kiện: trao thưởng, cập nhật quest state để người chơi có thể
  bắt đầu quest kế tiếp, đồng thời gửi `QuestCompletionResponse` (F60D).

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 8 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xF6 | Packet header - packet type identifier |
| 3 | 1 | Byte | 0x0D | Packet header - sub packet type identifier |
| 4 | 2 | ShortLittleEndian |  | QuestNumber |
| 6 | 2 | ShortLittleEndian |  | QuestGroup |
