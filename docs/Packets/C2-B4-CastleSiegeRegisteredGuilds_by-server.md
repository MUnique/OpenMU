# C2 B4 - CastleSiegeRegisteredGuilds (by server)

## Is sent when

After a guild master requested the list of registered guilds for castle siege.

## Causes the following actions on the client side

The client displays the list of registered guilds with their submitted marks.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0xB4  | Packet header - packet type identifier |
| 4 | 1 | Byte |  | GuildCount; The number of registered guilds. |
| 5 | RegisteredGuild.Length * GuildCount | Array of RegisteredGuild |  | Guilds |

### RegisteredGuild Structure

Contains the data of one registered guild for castle siege.

Length: 13 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 8 | String |  | GuildName |
| 8 | 4 | IntegerLittleEndian |  | MarksSubmitted; The number of guild marks submitted by this guild's alliance. |
| 12 | 1 | Byte |  | IsAllianceMaster; 1 if this guild is the alliance master, 0 otherwise. |