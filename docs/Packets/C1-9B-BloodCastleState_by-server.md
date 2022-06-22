# C1 9B - BloodCastleState (by server)

## Is sent when

The state of a blood castle event is about to change.

## Causes the following actions on the client side

The client side shows a message about the changing state.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   13   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x9B  | Packet header - packet type identifier |
| 3 | 1 | Status |  | State |
| 4 | 2 | ShortLittleEndian |  | RemainSecond |
| 6 | 2 | ShortLittleEndian |  | MaxMonster |
| 8 | 2 | ShortLittleEndian |  | CurMonster |
| 10 | 2 | ShortLittleEndian |  | ItemOwner |
| 12 | 1 | Byte |  | ItemLevel |

### Status Enum

Defines the status of the event.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Started | The event has just started and is running. |
| 1 | GateNotDestroyed | The event is running, but the gate is not destroyed. |
| 2 | Ended | The event has ended. |
| 4 | GateDestroyed | The event is running and the gate is destroyed. |