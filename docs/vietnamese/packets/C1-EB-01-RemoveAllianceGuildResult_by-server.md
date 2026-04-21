# C1 EB 01 - RemoveAllianceGuildResult (server gửi)

## Được gửi khi nào

Chủ bang hội đã gửi tin nhắn yêu cầu đuổi bang hội ra khỏi liên minh và tin nhắn đó đã được xử lý.

## Hành động phía client

Danh sách bang hội được cập nhật tương ứng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 7 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xEB | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x01 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Boolean |  | Kết quả |
| 5 | 1 | GuildRelationshipRequestType |  | Loại yêu cầu |
| 6 | 1 | GuildRelationshipType |  | Loại mối quan hệ |

### Enum GuildRelationshipRequestType
Mô tả loại yêu cầu.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Undefined | Loại yêu cầu không xác định. |
| 1 | Join | Kiểu tham gia. |
| 2 | Leave | Kiểu nghỉ phép. |

### Enum GuildRelationshipType
Mô tả loại mối quan hệ giữa các bang hội.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Undefined | Loại mối quan hệ không xác định. |
| 1 | Alliance | Kiểu quan hệ liên minh. |
| 2 | Hostility | Kiểu quan hệ thù địch. |