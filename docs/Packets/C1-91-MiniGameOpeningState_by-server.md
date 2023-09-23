# C1 91 - MiniGameOpeningState (by server)

## Is sent when

The player requests to get the current opening state of a mini game event, by clicking on an ticket item.

## Causes the following actions on the client side

The opening state of the event (remaining entering time, etc.) is shown at the client.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   7   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x91  | Packet header - packet type identifier |
| 3 | 1 | MiniGameType |  | GameType |
| 4 | 1 | Byte |  | RemainingEnteringTimeMinutes |
| 5 | 1 | Byte |  | UserCount |
| 6 | 1 | Byte |  | RemainingEnteringTimeMinutesLow; Just used for Chaos Castle. In this case, this field contains the lower byte of the remaining minutes. For other event types, this field is not used. |

### MiniGameType Enum

Defines the type of the mini game.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Undefined | Undefined mini game type. |
| 1 | DevilSquare | The devil square mini game. |
| 2 | BloodCastle | The blood castle mini game. |
| 3 | CursedTemple | The cursed temple mini game. |
| 4 | ChaosCastle | The chaos castle mini game. |
| 5 | IllusionTemple | The illusion temple mini game. |
| 6 | Doppelganger | The doppelgänger mini game. |