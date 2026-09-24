# C1 BF 10 - DoppelgangerStateUpdate (by server)

## Is sent when

The state of the doppelganger event changed.

## Causes the following actions on the client side

When the event starts (state Playing), the client shows the doppelganger frame and a message box with the failure conditions.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBF  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x10  | Packet header - sub packet type identifier |
| 4 | 1 | DoppelgangerState |  | State |

### DoppelgangerState Enum

The state of the doppelganger event.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Waiting | The event is waiting for the party members to enter. |
| 1 | Ready | The entrance is closed and the event is about to start. |
| 2 | Playing | The event is running. |
| 3 | Ended | The event has ended. |