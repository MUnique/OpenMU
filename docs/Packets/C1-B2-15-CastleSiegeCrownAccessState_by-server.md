# C1 B2 15 - CastleSiegeCrownAccessState (by server)

## Is sent when

The server updates the access state of the castle crown during the siege.

## Causes the following actions on the client side

The client updates the crown access state and accumulated time display.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   9   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x15  | Packet header - sub packet type identifier |
| 4 | 1 | CastleSiegeCrownAccessStateType |  | State |
| 5 | 4 | IntegerLittleEndian |  | AccumulatedTimeMs |

### CastleSiegeCrownAccessStateType Enum

Defines the result or progress state of a crown-access attempt.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Started | Crown registration has started. |
| 1 | Succeeded | Crown registration succeeded. |
| 2 | Failed | Crown registration failed. |
| 3 | OccupiedByOtherPlayer | Another player is accessing the crown. |
| 4 | OccupiedByOtherSide | A player from another siege side is accessing the crown. |