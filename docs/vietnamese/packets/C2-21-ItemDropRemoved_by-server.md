# C2 21 - ItemDropRemoved (server gửi)

## Được gửi khi nào

Một vật phẩm bị rơi đã bị xóa khỏi mặt đất của bản đồ, ví dụ: khi nó hết thời gian chờ hoặc được chọn.

## Hành động phía client

Khách hàng loại bỏ vật phẩm khỏi mặt đất của bản đồ.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC2 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 2 | Short |  | Tiêu đề gói - chiều dài của gói |
| 3 | 1 | Byte | 0x21 | Tiêu đề gói - mã định danh loại gói |
| 4 | 1 | Byte |  | số lượng vật phẩm |
| 5 | DroppedItemId.Length * ItemCount | Array of DroppedItemId |  | Dữ liệu mục |

### Cấu trúc DroppedItemId
Chứa id của một mục bị rơi.

Độ dài: 2 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortBigEndian |  | Id |