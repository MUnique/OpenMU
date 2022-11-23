# C1 92 - UpdateMiniGameState (by server)

## Is sent when

The state of a mini game event is about to change in 30 seconds.

## Causes the following actions on the client side

The client side shows a message about the changing state.

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
| 0 | DevilSquareClosed | Is sent when the devil square is currently closed and will be opened. "You will enter Devil Square (x seconds from now)". |
| 1 | DevilSquareOpened | Is sent when the devil square game is currently opened for entrance and closing for entrance. "The gate of Devil Square will close down in x seconds". |
| 2 | DevilSquareRunning | Is sent when the devil square game is currently running and is about to end. "The gate of Devil Square is closing down (x seconds remaining)". |
| 3 | BloodCastleClosed | The blood castle game is closed. "Blood Castle Closing (in x seconds)". |
| 4 | BloodCastleOpened | The blood castle game is opened for entrance. "Blood Castle Infiltration (in x seconds)". |
| 5 | BloodCastleEnding | The blood castle game is ending. "Blood Castle ends (in x seconds)". |
| 6 | BloodCastleFinished | The blood castle game is finished. "Blood Castle Event shuts down (in x seconds)". |
| 7 | BloodCastleCongratulations | The blood castle game was finished successfully. "Congratulations". |
| 10 | ChaosCastleClosed | The chaos castle game is closed. "Chaos Castle Closing (in x seconds)". |
| 11 | ChaosCastleOpened | The chaos castle game is opened for entrance. "Chaos Castle Penetration (in x seconds)". |
| 12 | ChaosCastleEnding | The chaos castle game is ending. "Chaos Castle Event ends (in x seconds)". |
| 13 | ChaosCastleFinished | The chaos castle game is finished. Chaos Castle Event shuts down (in x seconds)". |