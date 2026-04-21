# C1 37 - TradeRequestResponse (client gửi)

## Được gửi khi nào

Một người chơi được yêu cầu đã đáp lại yêu cầu giao dịch của một người chơi khác.

## Hành động phía server

Khi yêu cầu giao dịch được chấp nhận, máy chủ sẽ cố gắng mở một giao dịch mới và gửi phản hồi tương ứng cho cả hai người chơi.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x37 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Boolean |  | Giao dịch được chấp nhận |