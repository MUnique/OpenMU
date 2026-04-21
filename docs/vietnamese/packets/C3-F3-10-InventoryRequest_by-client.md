# C3 F3 10 - InventoryRequest (client gửi)

## Được gửi khi nào

Người chơi mua hoặc bán một vật phẩm thông qua cửa hàng cá nhân của mình.

## Hành động phía server

Máy chủ gửi danh sách hàng tồn kho trở lại máy khách.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF3 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x10 | Tiêu đề gói - mã định danh loại gói phụ |