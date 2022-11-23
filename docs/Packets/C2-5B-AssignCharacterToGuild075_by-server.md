# C2 5B - AssignCharacterToGuild075 (by server)

## Is sent when

The server wants to visibly assign a player to a guild, e.g. when two players met each other and one of them is a guild member.

## Causes the following actions on the client side

The players which belong to the guild are shown as guild players. If the game client doesn't met a player of this guild yet, it will send another request to get the guild information.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0x5B  | Packet header - packet type identifier |
| 4 | 1 | Byte |  | PlayerCount |
| 5 | GuildMemberRelation.Length * PlayerCount | Array of GuildMemberRelation |  | Members |

### GuildMemberRelation Structure

Relation between a guild and a member.

Length: 4 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortBigEndian |  | PlayerId |
| 2 | 2 | ShortBigEndian |  | GuildId |