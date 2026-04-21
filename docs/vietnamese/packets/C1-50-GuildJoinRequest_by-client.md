# C1 50 - GuildJoinRequest (client gửi)

## Được gửi khi nào

Khi người chơi (chưa có guild) gửi yêu cầu gia nhập guild.

## Hành động phía server

Server chuyển yêu cầu cho guild master. Mỗi thời điểm chỉ có một yêu cầu đang mở.
Nếu guild master đang có yêu cầu khác, server gửi phản hồi thất bại ngay cho
người yêu cầu.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0x50 | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | GuildMasterPlayerId |
