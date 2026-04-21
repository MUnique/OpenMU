# C1 28 - ItemRemoved (server gửi)

## Được gửi khi nào

Vật phẩm đã bị xóa khỏi kho của người chơi.

## Hành động phía client

Khách hàng xóa mục trong giao diện người dùng kho.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x28 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte |  | Khe hàng tồn kho; Vị trí bị ảnh hưởng của vật phẩm trong kho. |
| 4 | 1 | Byte | 1 | TrueFlag |