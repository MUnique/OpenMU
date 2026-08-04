# C1 B7 01 - CastleSiegeMachineUseResult (by server)

## Is sent when

After the player fired a siege machine (catapult).

## Causes the following actions on the client side

The client shows the catapult animation toward the target area.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   10   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB7  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x01  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Result |
| 5 | 2 | ShortBigEndian |  | NpcIndex |
| 7 | 1 | CastleSiegeMachineType |  | MachineType |
| 8 | 1 | Byte |  | TargetX |
| 9 | 1 | Byte |  | TargetY |

### CastleSiegeMachineType Enum

Defines the side-specific Castle Siege catapult type.

| Value | Name | Description |
|-------|------|-------------|
| 1 | Attack | The attacking-side catapult. |
| 2 | Defense | The defending-side catapult. |