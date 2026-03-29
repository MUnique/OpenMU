# C2 E9 - AllianceList (by server)

## Is sent when

A player requested the alliance list dialog.

## Causes the following actions on the client side

The client shows the list of guilds in the alliance.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0xE9  | Packet header - packet type identifier |
| 4 | 1 | Byte |  | GuildCount |
| 5 | 1 | Boolean |  | Success |
| 6 | 1 | Byte |  | __RivalCount |
| 7 | 1 | Byte |  | __UnionCount |
| 8 | AllianceGuildEntry.Length * GuildCount | Array of AllianceGuildEntry |  | Guilds |

### AllianceGuildEntry Structure

Contains the data of one alliance guild entry.

Length: 41 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte |  | MemberCount |
| 1 | 32 | Binary |  | Logo |
| 33 | 8 | String |  | GuildName |