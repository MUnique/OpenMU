# C4 C5 - LetterSendRequest (client gửi)

## Được gửi khi nào

Một người chơi muốn gửi thư cho nhân vật của người chơi khác.

## Hành động phía server

Bức thư sẽ được gửi cho nhân vật khác nếu nó tồn tại và người chơi có đủ số tiền cần thiết.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC4 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 2 | Short |  | Tiêu đề gói - chiều dài của gói |
| 3 | 1 | Byte | 0xC5 | Tiêu đề gói - mã định danh loại gói |
| 4 | 4 | IntegerLittleEndian |  | Id chữ cái |
| 8 | 10 | String |  | Người nhận |
| 18 | 60 | String |  | Tiêu đề |
| 78 | 1 | Byte |  | Xoay |
| 79 | 1 | Byte |  | Hoạt hình |
| 80 | 2 | ShortLittleEndian |  | Độ dài tin nhắn |
| 82 |  | String |  | Tin nhắn |