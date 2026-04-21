# C1 3F 3 - PlayerShopClosed (server gửi)

## Được gửi khi nào

Sau khi người chơi trong phạm vi yêu cầu đóng cửa hàng của mình hoặc sau khi tất cả vật phẩm đã được bán.

## Hành động phía client

Cửa hàng người chơi không còn hiển thị là mở nữa.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 7 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x3F | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x3 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Boolean | true | Thành công |
| 5 | 2 | ShortBigEndian |  | Id người chơi |