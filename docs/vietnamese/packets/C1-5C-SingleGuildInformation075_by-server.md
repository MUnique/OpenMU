# C1 5C - SingleGuildInformation075 (server gửi)

## Được gửi khi nào

Sau khi một guild được tạo. Tuy nhiên, trong OpenMU ta chỉ gửi message GuildInformations075 vì hoạt động tương đương.

## Hành động phía client

Các người chơi thuộc guild được hiển thị là người chơi guild.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 45 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0x5C | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | GuildId |
| 5 | 8 | String |  | GuildName |
| 13 | 32 | Binary |  | Logo |
