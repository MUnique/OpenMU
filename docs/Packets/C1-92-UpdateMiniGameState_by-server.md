# C1 92 - UpdateMiniGameState (by server)

## Is sent when

The state of a mini game event changed.

## Causes the following actions on the client side

The state of the mini game changes on the client side.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x92  | Packet header - packet type identifier |
| 3 | 1 | MiniGameTypeState |  | State |

### MiniGameTypeState Enum

The state of the mini games.

| Value | Name | Description |
|-------|------|-------------|
| 0 | DevilSquareClosed | The devil square game is closed. |
| 1 | DevilSquareOpened | The devil square game is opened for entrance. |
| 2 | DevilSquareRunning | The devil square game is running. |
| 3 | BloodCastleClosed | The blood castle game is closed. |
| 4 | BloodCastleOpened | The blood castle game is opened for entrance. |
| 5 | BloodCastleEnding | The blood castle game is ending. |
| 6 | BloodCastleFinished | The blood castle game is finished. |
| 10 | ChaosCastleClosed | The chaos castle game is closed. |
| 11 | ChaosCastleOpened | The chaos castle game is opened for entrance. |
| 12 | ChaosCastleEnding | The chaos castle game is ending. |
| 13 | ChaosCastleFinished | The chaos castle game is finished. |