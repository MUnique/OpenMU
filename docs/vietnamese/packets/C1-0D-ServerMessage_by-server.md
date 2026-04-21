# C1 0D - ServerMessage (server gửi)

## Được gửi khi nào



## Hành động phía client



## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte |  | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x0D | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | MessageType |  | Type |
| 4 |  | String |  | Tin nhắn |

### Enum MessageType
Xác định một loại tin nhắn máy chủ.

| Value | Name | Description |
|-------|------|-------------|
| 0 | GoldenCenter | Thông báo được hiển thị dưới dạng thông báo vàng ở giữa trong ứng dụng khách. |
| 1 | BlueNormal | Thông báo được hiển thị dưới dạng thông báo hệ thống màu xanh. |
| 2 | GuildNotice | Tin nhắn là thông báo của bang hội, được căn giữa bằng màu xanh lá cây. |