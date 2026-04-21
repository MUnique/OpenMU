# C3 1D - AreaSkillHit095 (client gửi)

## Được gửi khi nào

Một skill vùng đã được thực hiện và client quyết định trúng một hoặc nhiều mục tiêu.

## Hành động phía server

Server tính sát thương và áp dụng lên các mục tiêu. Người tấn công nhận response với sát thương gây ra.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte |  | Packet header - độ dài packet |
| 2 | 1 | Byte | 0x1D | Packet header - packet type identifier |
| 3 | 1 | Byte |  | SkillIndex; Chỉ số skill trong danh sách skill. |
| 4 | 1 | Byte |  | TargetX |
| 5 | 1 | Byte |  | TargetY |
| 6 | 1 | Byte |  | Counter |
| 7 | 1 | Byte |  | TargetCount; Số lượng mục tiêu bị trúng. |
| 8 | TargetData.Length * TargetCount | Array of TargetData |  | Targets |

### Cấu trúc TargetData

Chứa dữ liệu của mục tiêu.

Độ dài: 2 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortBigEndian |  | TargetId |
