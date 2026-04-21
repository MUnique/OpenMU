# C3 36 - TradeRequest (client gửi)

## Được gửi khi nào

Người chơi yêu cầu mở giao dịch với người chơi khác.

## Hành động phía server

Yêu cầu được chuyển tiếp đến người chơi được yêu cầu.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x36 | Tiêu đề gói - mã định danh loại gói |
| 3 | 2 | ShortBigEndian |  | Id người chơi |