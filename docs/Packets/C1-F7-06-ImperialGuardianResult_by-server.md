# C1 F7 06 - ImperialGuardianResult (by server)

## Is sent when

A zone of the imperial guardian event has been cleared, or the event ended.

## Causes the following actions on the client side

The client shows the result and hides the timer.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   12   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF7  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x06  | Packet header - sub packet type identifier |
| 4 | 1 | ResultType |  | Result; The result. |
| 8 | 4 | IntegerLittleEndian |  | Experience; The rewarded experience, when the event has been completed. The field is aligned to 4 bytes, because the client structure is not packed. |

### ResultType Enum

The result of the imperial guardian event.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Failed | The event failed. |
| 1 | ZoneCleared | The zone has been cleared. |
| 2 | Success | The event has been completed. |