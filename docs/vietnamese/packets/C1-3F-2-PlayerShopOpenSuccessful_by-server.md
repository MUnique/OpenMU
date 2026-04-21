# C1 3F 2 - PlayerShopOpenSuccessful (server gửi)

## Được gửi khi nào

Sau khi người chơi yêu cầu mở cửa hàng của mình và yêu cầu này đã thành công.

## Hành động phía client

Cửa hàng người chơi riêng được hiển thị là đang mở.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x3F | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x2 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Boolean | true | Thành công |