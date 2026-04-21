# C1 E5 - GuildRelationshipRequest (server gửi)

## Được gửi khi nào

Chủ bang hội đã gửi yêu cầu thay đổi mối quan hệ (liên minh hoặc thù địch) và máy chủ sẽ chuyển tiếp yêu cầu này đến chủ bang hội mục tiêu.

## Hành động phía client

Chủ bang hội mục tiêu (người nhận tin nhắn này) nhìn thấy hộp thoại yêu cầu gửi đến.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 7 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xE5 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | GuildRelationshipType |  | Loại mối quan hệ |
| 4 | 1 | GuildRelationshipRequestType |  | Loại yêu cầu |
| 5 | 2 | ShortBigEndian |  | Id người gửi |

### Enum GuildRelationshipType
Mô tả loại mối quan hệ giữa các bang hội.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Undefined | Loại mối quan hệ không xác định. |
| 1 | Alliance | Kiểu quan hệ liên minh. |
| 2 | Hostility | Kiểu quan hệ thù địch. |

### Enum GuildRelationshipRequestType
Mô tả loại yêu cầu.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Undefined | Loại yêu cầu không xác định. |
| 1 | Join | Kiểu tham gia. |
| 2 | Leave | Kiểu nghỉ phép. |