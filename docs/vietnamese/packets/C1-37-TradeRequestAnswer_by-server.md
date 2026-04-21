# C1 37 - TradeRequestAnswer (server gửi)

## Được gửi khi nào

Người chơi nhận được tin nhắn này đã gửi yêu cầu giao dịch cho người chơi khác. Tin nhắn này được gửi khi người chơi khác phản hồi yêu cầu này.

## Hành động phía client

Nếu giao dịch được chấp nhận, hộp thoại giao dịch sẽ mở ra. Nếu không, một thông báo sẽ được hiển thị.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 20 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x37 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Boolean |  | Đã chấp nhận |
| 4 | 10 | String |  | Name |
| 14 | 2 | ShortBigEndian |  | Cấp độ đối tác thương mại |
| 16 | 4 | IntegerLittleEndian |  | Id bang hội |