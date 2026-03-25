# C1 E9 - AllianceList (by server)

## Is sent when

A player requested the alliance list dialog.

## Causes the following actions on the client side

The client shows the list of guilds in the alliance.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xE9  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | GuildCount |
| 4 | AllianceGuildEntry.Length * GuildCount | Array of AllianceGuildEntry |  | Guilds |

### AllianceGuildEntry Structure

Contains the data of one alliance guild entry.

Length: 13 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 4 | IntegerLittleEndian |  | GuildId |
| 4 | 9 | String |  | GuildName |