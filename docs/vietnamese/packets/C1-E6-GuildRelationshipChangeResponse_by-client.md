# C1 E6 - GuildRelationshipChangeResponse (client gửi)

## Được gửi khi nào

Một chủ bang hội đã trả lời yêu cầu của một chủ bang hội khác về việc thay đổi mối quan hệ giữa các bang hội của họ.

## Hành động phía server

Máy chủ sẽ gửi phản hồi lại cho người yêu cầu. Nếu chủ bang hội đồng ý, họ sẽ thực hiện các hành động cần thiết.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 8 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xE6 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | GuildRelationshipType |  | Loại mối quan hệ |
| 4 | 1 | GuildRequestType |  | Loại yêu cầu |
| 5 | 1 | Boolean |  | Phản ứng |
| 6 | 2 | ShortBigEndian |  | Id người chơi mục tiêu |

### Enum GuildRelationshipType
Mô tả loại mối quan hệ giữa các bang hội.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Undefined | Loại mối quan hệ không xác định. |
| 1 | Alliance | Kiểu quan hệ liên minh. |
| 2 | Hostility | Kiểu quan hệ thù địch. |

### Enum GuildRequestType
Mô tả loại yêu cầu.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Undefined | Loại yêu cầu không xác định. |
| 1 | Join | Kiểu tham gia. |
| 2 | Leave | Kiểu nghỉ phép. |