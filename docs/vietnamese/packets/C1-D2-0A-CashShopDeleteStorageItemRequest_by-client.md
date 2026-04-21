# C1 D2 0A - CashShopDeleteStorageItemRequest (client gửi)

## Được gửi khi nào

Người chơi muốn xóa một vật phẩm trong kho lưu trữ của cửa hàng tiền mặt.

## Hành động phía server

Máy chủ xóa vật phẩm khỏi kho lưu trữ của cửa hàng tiền mặt.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 234 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xD2 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x0A | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 4 | IntegerLittleEndian |  | Mã cơ sở |
| 8 | 4 | IntegerLittleEndian |  | Mã sản phẩm chính |
| 12 | 1 | Byte |  | Loại sản phẩm |