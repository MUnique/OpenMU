# C1 F6 0B - QuestProceedRequest (client gửi)

## Được gửi khi nào

Sau khi server bắt đầu quest (gửi message F60B), client gửi request để tiếp tục
quest.

## Hành động phía server

Server cập nhật quest state tương ứng.
Phản hồi tiếp theo phụ thuộc cấu hình quest:

- gửi `QuestProgress` (F60C), hoặc
- gửi lại message bắt đầu quest (F60B).

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 9 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xF6 | Packet header - packet type identifier |
| 3 | 1 | Byte | 0x0B | Packet header - sub packet type identifier |
| 4 | 2 | ShortLittleEndian |  | QuestNumber |
| 6 | 2 | ShortLittleEndian |  | QuestGroup |
| 8 | 1 | QuestProceedAction |  | ProceedAction |

### Enum QuestProceedAction

Mô tả cách tiếp tục quest đã chỉ định.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Undefined | Hành động không xác định. |
| 1 | AcceptQuest | Chấp nhận và bắt đầu quest. |
| 2 | RefuseQuest | Từ chối quest và không bắt đầu. |
