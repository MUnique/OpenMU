# C1 52 - GuildListRequest (client gửi)

## Được gửi khi nào

Người chơi bang hội mở menu bang hội của mình trong ứng dụng khách trò chơi.

## Hành động phía server

Danh sách tất cả các thành viên bang hội và trạng thái của họ sẽ được gửi lại dưới dạng phản hồi.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 3 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x52 | Tiêu đề gói - mã định danh loại gói |