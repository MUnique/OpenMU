# C2 65 - AssignCharacterToGuild (by server)

## Is sent when

The server wants to visibly assign a player to a guild, e.g. when two players met each other and one of them is a guild member.

## Causes the following actions on the client side

The players which belong to the guild are shown as guild players. If the game client doesn't met a player of this guild yet, it will send another request to get the guild information.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0x65  | Packet header - packet type identifier |
| 4 | 1 | Byte |  | PlayerCount |
| 5 | GuildMemberRelation.Length * PlayerCount | Array of GuildMemberRelation |  | Members |

### GuildMemberRelation Structure

Relation between a guild and a member.

Length: 12 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 4 | IntegerLittleEndian |  | GuildId |
| 4 | 1 | GuildMemberRole |  | Role |
| 7 << 7 | 1 bit | Boolean |  | IsPlayerAppearingNew |
| 7 | 2 | ShortBigEndian |  | PlayerId |

### GuildMemberRole Enum

Defines the role of a guild member.

| Value | Name | Description |
|-------|------|-------------|
| 0 | NormalMember | The member is a normal member without special rights. |
| 32 | BattleMaster | The member is a battle master. |
| 128 | GuildMaster | The member is the guild master. |
| 255 | Undefined | The character is not a member, therefore the role is undefined. |