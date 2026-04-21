# C1 3F 12 - ClosePlayerShopDialog (server gửi)

## Được gửi khi nào

Sau khi người chơi yêu cầu đóng cửa hàng hoặc sau khi tất cả vật phẩm đã được bán hết.

## Hành động phía client

Hộp thoại cửa hàng người chơi sẽ đóng đối với cửa hàng của người chơi được chỉ định.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 6 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x3F | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x12 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 2 | ShortBigEndian |  | Id người chơi |