# C1 66 - GuildInformation (by server)

## Is sent when

A game client requested the (public) info of a guild, e.g. when it met a player of previously unknown guild.

## Causes the following actions on the client side

The players which belong to the guild are shown as guild players.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   60   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x66  | Packet header - packet type identifier |
| 4 | 4 | IntegerLittleEndian |  | GuildId |
| 8 | 1 | Byte |  | GuildType |
| 9 | 8 | String |  | AllianceGuildName |
| 17 | 8 | String |  | GuildName |
| 25 | 32 | Binary |  | Logo |