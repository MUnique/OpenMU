# C1 B2 1D - CastleSiegeGuildCommand (by server)

## Is sent when

An alliance master sends a command during the Castle Siege battle.

## Causes the following actions on the client side

The client shows the command on the mini map.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   8   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x1D  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Team; Team number from 0 to 7. |
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