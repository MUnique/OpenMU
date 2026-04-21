# C1 53 - GuildKickPlayerRequest (client gửi)

## Được gửi khi nào

Khi:

- thành viên guild muốn rời guild (tự kick chính mình), hoặc
- guild master muốn kick thành viên khác khỏi guild.

## Hành động phía server

Nếu người gửi có quyền kick, server xóa người bị kick khỏi guild.
Nếu guild master tự kick chính mình, guild sẽ bị giải tán.
Server gửi các phản hồi tương ứng cho các người chơi liên quan.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte |  | Packet header - độ dài packet |
| 2 | 1 | Byte | 0x53 | Packet header - packet type identifier |
| 3 | 10 | String |  | PlayerName |
| 13 |  | String |  | SecurityCode |
