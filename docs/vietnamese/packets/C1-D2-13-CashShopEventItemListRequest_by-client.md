# C1 D2 13 - CashShopEventItemListRequest (client gửi)

## Được gửi khi nào

Khi người chơi muốn xem qua danh sách vật phẩm sự kiện.

## Hành động phía server

Máy chủ sẽ gửi lại danh sách các mục sự kiện.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 8 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xD2 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x13 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 4 | IntegerLittleEndian |  | Danh mụcChỉ mục |