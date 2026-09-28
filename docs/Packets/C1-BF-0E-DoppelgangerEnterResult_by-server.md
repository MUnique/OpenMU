# C1 BF 0E - DoppelgangerEnterResult (by server)

## Is sent when

The player requested to enter the doppelganger event through the NPC Lugard.

## Causes the following actions on the client side

On failure, the client locks the enter button of the doppelganger entry dialog and may show a message.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBF  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x0E  | Packet header - sub packet type identifier |
| 4 | 1 | EnterResult |  | Result |

### EnterResult Enum

Result of the doppelganger enter request.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Success | The player entered the event. |
| 1 | Failed | The player could not enter, e.g. because of a missing ticket item. The client locks the enter button without showing a message. |
| 2 | AlreadyStarted | The event is already running or occupied by another party. The client shows "Battle has already commenced. You cannot enter.". |
| 3 | PlayerKiller | Player killers are not allowed to enter. The client shows "You cannot enter if you are a 1st Stage Outlaw.". |
| 4 | EntranceAvailable | The client unlocks the enter button of the doppelganger entry dialog. |