# C1 C5 - LetterSendResponse (server gửi)

## Được gửi khi nào

Sau khi người chơi yêu cầu gửi thư cho người chơi khác.

## Hành động phía client

Tùy thuộc vào kết quả, hộp thoại gửi thư sẽ đóng hoặc xuất hiện thông báo lỗi.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 8 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xC5 | Tiêu đề gói - mã định danh loại gói |
| 4 | 4 | IntegerLittleEndian |  | Id chữ cái |
| 3 | 1 | LetterSendRequestResult |  | Kết quả |

### Enum LetterSendRequestResult
Mô tả kết quả của một yêu cầu gửi thư.

| Value | Name | Description |
|-------|------|-------------|
| 0 | TryAgain | Bức thư không được gửi vì có vấn đề nội bộ. Người dùng nên thử lại. |
| 1 | Success | Bức thư đã được gửi đi. |
| 2 | MailboxFull | Hộp thư của người nhận đã đầy. |
| 3 | ReceiverNotExists | Người nhận không tồn tại. |
| 4 | CantSendToYourself | Một lá thư không thể được gửi cho chính bạn. |
| 7 | NotEnoughMoney | Người gửi không có đủ tiền để gửi thư. |