# C3 3F 01 - PlayerShopSetItemPrice (client gửi)

## Được gửi khi nào

Người chơi muốn đặt giá cho một vật phẩm có trong cửa hàng vật phẩm cá nhân của mình.

## Hành động phía server

Giá được đặt cho mặt hàng được chỉ định. Chỉ hoạt động nếu cửa hàng hiện đang đóng cửa.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 9 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x3F | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x01 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Byte |  | Khe vật phẩm |
| 5 | 4 | IntegerLittleEndian |  | Giá |