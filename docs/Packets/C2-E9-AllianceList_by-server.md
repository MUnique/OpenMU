# C2 E9 - AllianceList (by server)

## Is sent when

After a player requested the list of guilds in their alliance.

## Causes the following actions on the client side

The client displays the alliance guild list.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0xE9  | Packet header - packet type identifier |
| 4 | 1 | Byte |  | GuildCount; The number of guilds in the alliance. |
| 5 | AllianceGuild.Length * GuildCount | Array of AllianceGuild |  | Guilds |

### AllianceGuild Structure

Information about one guild in the alliance.

Length: 9 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 8 | String |  | GuildName; The name of the guild (8 characters, space-padded). |
| 8 | 1 | Byte |  | IsMasterGuild; 1 if this guild is the alliance master, 0 otherwise. |