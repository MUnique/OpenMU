# C1 F3 11 - SkillListUpdate075 (server gửi)

## Được gửi khi nào

Thông thường khi người chơi vào game với một nhân vật. Khi skill được thêm hoặc gỡ, message này cũng được gửi, nhưng với giá trị count dễ gây hiểu nhầm.

## Hành động phía client

Danh sách skill được khởi tạo.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte |  | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xF3 | Packet header - packet type identifier |
| 3 | 1 | Byte | 0x11 | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Count; Dùng lẫn lộn: số lượng skill trong danh sách (khi là danh sách). 0xFE khi thêm skill, 0xFF khi gỡ skill. |
| 5 | SkillEntry.Length * Count | Array of SkillEntry |  | Skills |

### Cấu trúc SkillEntry

Cấu trúc cho một mục trong danh sách skill.

Độ dài: 3 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte |  | SkillIndex |
| 1 | 2 | ShortBigEndian |  | SkillNumberAndLevel |
