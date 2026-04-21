# C1 14 - MapObjectOutOfScope (server gửi)

## Được gửi khi nào

Một hoặc nhiều đối tượng (người chơi, npc, v.v.) trên bản đồ đã nằm ngoài phạm vi, ví dụ: khi người chơi của chính họ di chuyển ra khỏi nó/họ hoặc chính đồ vật đó đã di chuyển.

## Hành động phía client

Ứng dụng khách trò chơi sẽ xóa các đối tượng khỏi bản đồ trò chơi.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte |  | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x14 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte |  | Số lượng đối tượng |
| 4 | ObjectId.Length * ObjectCount | Array of ObjectId |  | Đối tượng |

### Cấu trúc ObjectId
Chứa id của một đối tượng.

Độ dài: 2 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortBigEndian |  | Id |