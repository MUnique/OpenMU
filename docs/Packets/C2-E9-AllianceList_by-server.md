# C2 E9 - AllianceList (by server)

## Is sent when

After the player requested the alliance list.

## Causes the following actions on the client side

The client shows the list of guilds in the alliance.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0xE9  | Packet header - packet type identifier |
| 4 | 1 | Byte |  | GuildCount |
| 5 | AllianceGuildEntry.Length * GuildCount | Array of AllianceGuildEntry |  | Guilds |

### AllianceGuildEntry Structure

Information about one guild in the alliance.

Length: 19 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 8 | String |  | GuildName |
| 8 | 1 | Byte |  | MemberCount |
| 9 | 10 | String |  | GuildMasterName |