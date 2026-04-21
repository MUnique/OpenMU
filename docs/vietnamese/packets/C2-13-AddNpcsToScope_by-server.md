# C2 13 - AddNpcsToScope (server gửi)

## Được gửi khi nào

Một hoặc nhiều NPC lọt vào phạm vi quan sát của người chơi.

## Hành động phía client

Máy khách thêm NPC vào bản đồ được hiển thị.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC2 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 2 | Short |  | Tiêu đề gói - chiều dài của gói |
| 3 | 1 | Byte | 0x13 | Tiêu đề gói - mã định danh loại gói |
| 4 | 1 | Byte |  | NpcCount |
| 5 | NpcData.Length * NpcCount | Array of NpcData |  | NPC |

### Cấu trúc NpcData
Chứa dữ liệu của NPC.

Độ dài: 10 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortBigEndian |  | Id |
| 2 | 2 | ShortBigEndian |  | LoạiSố |
| 4 | 1 | Byte |  | Vị trí hiện tạiX |
| 5 | 1 | Byte |  | Vị trí hiện tạiY |
| 6 | 1 | Byte |  | Vị trí mục tiêuX |
| 7 | 1 | Byte |  | Vị trí mục tiêuY |
| 8 | 4 bit | Byte |  | Xoay |
| 9 | 1 | Byte |  | Đếm hiệu ứng; Xác định số lượng hiệu ứng sẽ được gửi sau trường này. Điều này hiện không được hỗ trợ. |