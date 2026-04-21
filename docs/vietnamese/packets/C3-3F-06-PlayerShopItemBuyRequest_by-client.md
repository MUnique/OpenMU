# C3 3F 06 - PlayerShopItemBuyRequest (client gửi)

## Được gửi khi nào

Một người chơi muốn mua vật phẩm của cửa hàng người chơi khác.

## Hành động phía server

Nếu người mua có đủ tiền thì vật phẩm sẽ được bán cho người chơi. Cả hai người chơi sẽ nhận được thông báo về điều đó.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 17 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x3F | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x06 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 2 | ShortBigEndian |  | Id người chơi |
| 6 | 10 | String |  | Tên người chơi |
| 16 | 1 | Byte |  | Khe vật phẩm |