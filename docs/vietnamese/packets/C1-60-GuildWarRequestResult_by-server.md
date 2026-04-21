# C1 60 - GuildWarRequestResult (server gửi)

## Được gửi khi nào

Một chủ bang hội đã yêu cầu một cuộc chiến bang hội chống lại một bang hội khác.

## Hành động phía client

Chủ bang hội của bang hội khác nhận được yêu cầu này.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x60 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | RequestResult |  | Kết quả |

### Enum RequestResult
Mô tả kết quả của yêu cầu chiến tranh bang hội.

| Value | Name | Description |
|-------|------|-------------|
| 0 | GuildNotFound | Thất bại, không tìm thấy bang hội. |
| 1 | RequestSentToGuildMaster | Yêu cầu đã được gửi đến chủ bang hội. Đang chờ câu trả lời của anh. |
| 2 | GuildMasterOffline | Không thành công vì chủ bang hội đang ngoại tuyến. |
| 3 | NotInGuild | Không thành công vì người chơi không ở trong bang hội. |
| 4 | Failed | Cuộc chiến bang hội không thể bắt đầu, ví dụ: vì sân bóng đá đã được sử dụng rồi. |
| 5 | NotTheGuildMaster | Không thành công vì người chơi không phải là chủ bang hội. |
| 6 | AlreadyInWar | Không thành công vì bang hội được yêu cầu đã tham gia chiến tranh. |