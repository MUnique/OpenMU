# C1 3F 08 - PlayerShopItemSoldToPlayer (server gửi)

## Được gửi khi nào

Một vật phẩm trong cửa hàng của người chơi đã được bán cho người chơi khác.

## Hành động phía client

Vật phẩm sẽ bị xóa khỏi kho đồ của người chơi và thông báo hệ thống màu xanh lam sẽ xuất hiện.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 15 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x3F | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x08 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Byte |  | Khe hàng tồn kho |
| 5 | 10 | String |  | Tên người mua |