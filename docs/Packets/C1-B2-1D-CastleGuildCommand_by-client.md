# C1 B2 1D - CastleGuildCommand (by client)

## Is sent when

The guild master sent a command to his guild during the castle siege event.

## Causes the following actions on the server side

The command is shown on the mini map of the guild members.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   8   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x1D  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Team; Team Number 0 to 7. |
| 5 | 1 | Byte |  | PositionX |
| 6 | 1 | Byte |  | PositionY |
| 7 | 1 | CastleSiegeGuildCommandType |  | Command |

### CastleSiegeGuildCommandType Enum

Defines a command placed on the Castle Siege mini-map.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Attack | Orders the team to attack. |
| 1 | Defend | Orders the team to defend. |
| 2 | Wait | Orders the team to wait. |