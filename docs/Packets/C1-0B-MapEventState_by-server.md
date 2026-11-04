# C1 0B - MapEventState (by server)

## Is sent when

The state of event is about to change.

## Causes the following actions on the client side

The event's effect is shown.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x0B  | Packet header - packet type identifier |
| 3 | 1 | Boolean |  | Enable |
| 4 | 1 | Events |  | Event |

### Events Enum

Defines all events.

| Value | Name | Description |
|-------|------|-------------|
| 1 | RedDragon | Red dragon invasion. |
| 3 | GoldenDragon | Golden dragon invasion. |