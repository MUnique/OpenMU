# C2 B4 - CastleSiegeRegisteredGuildList (by server)

## Is sent when

After the guild master requested the list of guilds registered for the next siege.

## Causes the following actions on the client side

The client shows the list of registered guilds and their mark counts.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0xB4  | Packet header - packet type identifier |
| 4 | 4 | IntegerBigEndian |  | GuildCount |
| 8 | RegisteredGuildEntry.Length * GuildCount | Array of RegisteredGuildEntry |  | Guilds |

### RegisteredGuildEntry Structure

Information about one guild registered for the next castle siege.

Length: 17 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 8 | String |  | GuildName |
| 8 | 4 | IntegerBigEndian |  | GuildMarkCount |
| 12 | 4 | IntegerBigEndian |  | RegistrationId |
| 16 | 1 | Boolean |  | IsCastleOwner |