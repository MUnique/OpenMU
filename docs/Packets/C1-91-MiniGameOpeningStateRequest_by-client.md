# C1 91 - MiniGameOpeningStateRequest (by client)

## Is sent when

The player requests to get the remaining time of the currently entered event.

## Causes the following actions on the server side

The remaining time is sent back to the client.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x91  | Packet header - packet type identifier |
| 3 | 1 | MiniGameType |  | EventType |
| 4 | 1 | Byte |  | EventLevel |

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