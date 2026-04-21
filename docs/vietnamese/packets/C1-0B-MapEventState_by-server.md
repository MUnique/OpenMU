# C1 0B - MapEventState (server gửi)

## Được gửi khi nào

Tình trạng của sự kiện sắp thay đổi.

## Hành động phía client

Hiệu ứng của sự kiện được hiển thị.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x0B | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Boolean |  | Cho phép |
| 4 | 1 | Events |  | Sự kiện |

### Enum Events
Xác định tất cả các sự kiện.

| Value | Name | Description |
|-------|------|-------------|
| 1 | RedDragon | Cuộc xâm lược của rồng đỏ. |
| 3 | GoldenDragon | Cuộc xâm lược của rồng vàng. |