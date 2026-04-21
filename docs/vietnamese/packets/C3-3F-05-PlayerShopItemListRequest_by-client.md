# C3 3F 05 - PlayerShopItemListRequest (client gửi)

## Được gửi khi nào

Một người chơi mở cửa hàng của người chơi khác.

## Hành động phía server

Danh sách vật phẩm sẽ được gửi lại nếu cửa hàng của người chơi hiện đang mở.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 16 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x3F | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x05 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 2 | ShortBigEndian |  | Id người chơi |
| 6 | 10 | String |  | Tên người chơi |