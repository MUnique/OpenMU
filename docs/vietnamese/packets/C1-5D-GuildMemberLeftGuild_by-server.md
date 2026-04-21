# C1 5D - GuildMemberLeftGuild (server gửi)

## Được gửi khi nào

Một người chơi đã rời khỏi bang hội. Tin nhắn này được gửi đến người chơi và tất cả những người chơi xung quanh.

## Hành động phía client

Người chơi không còn được hiển thị với tư cách là thành viên bang hội nữa.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x5D | Tiêu đề gói - mã định danh loại gói |
| 3 | 2 | ShortBigEndian |  | Id người chơi |
| 3 << 7 | 1 bit | Boolean |  | IsGuildMaster |