# C2 B5 - CastleSiegeGuildList (by server)

## Is sent when

After the guild master requested the list of all guilds in the current castle siege.

## Causes the following actions on the client side

The client shows the list of guilds participating in the castle siege.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0xB5  | Packet header - packet type identifier |
| 4 | 1 | Byte |  | Result |
| 5 | 4 | IntegerLittleEndian |  | GuildCount |
| 9 | CastleSiegeGuildEntry.Length * GuildCount | Array of CastleSiegeGuildEntry |  | Guilds |

### CastleSiegeGuildEntry Structure

Information about one guild in the castle siege.

Length: 14 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | CastleSiegeJoinSide |  | Side |
| 1 | 1 | Boolean |  | IsInvolved |
| 2 | 8 | String |  | GuildName |
| 10 | 4 | IntegerLittleEndian |  | Score |

### CastleSiegeJoinSide Enum

Defines a participant's side in the Castle Siege battle.

| Value | Name | Description |
|-------|------|-------------|
| 0 | None | The participant does not belong to a siege side. |
| 1 | Defense | The defending alliance. |
| 2 | Attack1 | The first attacking alliance. |
| 3 | Attack2 | The second attacking alliance. |
| 4 | Attack3 | The third attacking alliance. |