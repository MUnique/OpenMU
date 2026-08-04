# C1 B7 00 - CastleSiegeMachineInterface (by server)

## Is sent when

The server sends the siege machine interface to a player who is operating the machine.

## Causes the following actions on the client side

The client shows the siege machine operation interface.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   8   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB7  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x00  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Result |
| 5 | 1 | CastleSiegeMachineType |  | MachineType |
| 6 | 2 | ShortBigEndian |  | NpcIndex |

### CastleSiegeMachineType Enum

Defines the side-specific Castle Siege catapult type.

| Value | Name | Description |
|-------|------|-------------|
| 1 | Attack | The attacking-side catapult. |
| 2 | Defense | The defending-side catapult. |