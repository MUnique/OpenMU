# C3 36 - TradeRequest (server gửi)

## Được gửi khi nào

Một người chơi khác đã yêu cầu giao dịch.

## Hành động phía client

Hộp thoại yêu cầu giao dịch được hiển thị.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 13 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x36 | Tiêu đề gói - mã định danh loại gói |
| 3 | 10 | String |  | Name |