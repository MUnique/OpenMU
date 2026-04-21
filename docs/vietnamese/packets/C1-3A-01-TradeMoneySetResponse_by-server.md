# C1 3A 01 - TradeMoneySetResponse (server gửi)

## Được gửi khi nào

Tiền giao dịch đã được đặt theo yêu cầu trước đó của người chơi.

## Hành động phía client

Số tiền mà người chơi đặt vào giao dịch sẽ được cập nhật trên giao diện người dùng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x3A | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x01 | Tiêu đề gói - mã định danh loại gói phụ |