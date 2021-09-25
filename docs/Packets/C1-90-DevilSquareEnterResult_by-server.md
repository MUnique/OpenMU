# C1 90 - DevilSquareEnterResult (by server)

## Is sent when

The player requested to enter the devil square mini game through the Charon NPC.

## Causes the following actions on the client side

In case it failed, it shows the corresponding error message.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   3   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x90  | Packet header - packet type identifier |
| 3 | 1 | EnterResult |  | Result |
| 4 | 1 | Byte |  | TicketItemInventoryIndex |

### EnterResult Enum

Defines the result of the enter request.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Success | The event has been entered. |
| 1 | Failed | Entering the event failed, e.g. by missing event ticket or level range. |
| 2 | NotOpen | Entering the event failed, because it's not opened. |
| 3 | CharacterLevelTooHigh | Entering the event failed, because the character level is too high for the requested event level. |
| 4 | CharacterLevelTooLow | Entering the event failed, because the character level is too low for the requested event level. |
| 5 | Full | Entering the event failed, because it's full. |