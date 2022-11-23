# C2 5A - GuildInformations075 (by server)

## Is sent when

A player went into the scope of one or more guild members.

## Causes the following actions on the client side

The players which belong to the guild are shown as guild players.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0x5A  | Packet header - packet type identifier |
| 4 | 1 | Byte |  | GuildCount |
| 5 | GuildInfo.Length * GuildCount | Array of GuildInfo |  | Guilds |

### GuildInfo Structure

Information about one guild.

Length: 42 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortBigEndian |  | GuildId |
| 2 | 8 | String |  | GuildName |
| 10 | 32 | Binary |  | Logo |