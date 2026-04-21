# C1 BF 03 - IllusionTempleUserCount (server gửi)

## Được gửi khi nào

?

## Hành động phía client

Khách hàng hiển thị số lượng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 10 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xBF | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x03 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Byte |  | Số lượng người dùng1 |
| 5 | 1 | Byte |  | Số lượng người dùng2 |
| 6 | 1 | Byte |  | Số lượng người dùng3 |
| 7 | 1 | Byte |  | Số lượng người dùng4 |
| 8 | 1 | Byte |  | Số lượng người dùng5 |
| 9 | 1 | Byte |  | Số lượng người dùng6 |