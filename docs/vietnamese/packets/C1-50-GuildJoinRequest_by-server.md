# C1 50 - GuildJoinRequest (server gửi)

## Được gửi khi nào

Một người chơi đã yêu cầu gia nhập bang hội. Tin nhắn này sau đó được gửi đến chủ bang hội.

## Hành động phía client

Chủ bang hội nhận được một hộp tin nhắn với yêu cầu hiện lên.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x50 | Tiêu đề gói - mã định danh loại gói |
| 3 | 2 | ShortBigEndian |  | Id người yêu cầu |