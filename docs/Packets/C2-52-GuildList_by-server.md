# C2 52 - GuildList (by server)

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
| 13 | 8 | String |  | RivalGuildName |
| 24 | GuildMember.Length * GuildMemberCount | Array of GuildMember |  | Members |

### GuildMember Structure

Contains the data of one guild member.

Length: 13 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 10 | String |  | Name |
| 10 | 1 | Byte |  | ServerId |
| 11 | 1 | Byte |  | ServerId2 |
| 12 | 1 | GuildMemberRole |  | Role |

### GuildMemberRole Enum

Defines the role of a guild member.

| Value | Name | Description |
|-------|------|-------------|
| 0 | NormalMember | The member is a normal member without special rights. |
| 32 | BattleMaster | The member is a battle master. |
| 128 | GuildMaster | The member is the guild master. |
| 255 | Undefined | The character is not a member, therefore the role is undefined. |