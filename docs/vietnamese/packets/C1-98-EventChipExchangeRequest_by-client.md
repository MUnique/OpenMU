# C1 98 - EventChipExchangeRequest (client gửi)

## Được gửi khi nào

Người chơi yêu cầu đổi chip sự kiện sang thứ khác.

## Hành động phía server

Một phản hồi sẽ được gửi lại cho khách hàng cùng với kết quả trao đổi.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x98 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte |  | Type |