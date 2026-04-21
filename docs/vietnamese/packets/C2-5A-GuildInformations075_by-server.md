# C2 5A - GuildInformations075 (server gửi)

## Được gửi khi nào

Người chơi đi vào phạm vi của một hoặc nhiều thành viên guild.

## Hành động phía client

Các người chơi thuộc guild được hiển thị là người chơi guild.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC2 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 2 | Short |  | Packet header - độ dài packet |
| 3 | 1 | Byte | 0x5A | Packet header - packet type identifier |
| 4 | 1 | Byte |  | GuildCount |
| 5 | GuildInfo.Length * GuildCount | Array of GuildInfo |  | Guilds |

### Cấu trúc GuildInfo

Thông tin về một guild.

Độ dài: 42 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortBigEndian |  | GuildId |
| 2 | 8 | String |  | GuildName |
| 10 | 32 | Binary |  | Logo |
