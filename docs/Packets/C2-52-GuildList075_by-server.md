# C2 52 - GuildList075 (by server)

## Is sent when

After a game client requested the list of players of his guild, which is usually the case when the player opens the guild dialog at the game client.

## Causes the following actions on the client side

The list of player is available at the client.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0x52  | Packet header - packet type identifier |
| 4 | 1 | Boolean |  | IsInGuild |
| 5 | 1 | Byte |  | GuildMemberCount |
| 8 | 4 | IntegerLittleEndian |  | TotalScore |
| 12 | 1 | Byte |  | CurrentScore |
| 13 | GuildMember.Length * GuildMemberCount | Array of GuildMember |  | Members |

### GuildMember Structure

Contains the data of one guild member.

Length: 12 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 10 | String |  | Name |
| 10 | 1 | Byte |  | ServerId |
| 11 | 1 | Byte |  | ServerId2 |