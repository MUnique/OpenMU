# C1 D1 01 - KanturuEnterResult (by server)

## Is sent when

The player attempted to enter the Kanturu event through the gateway NPC.

## Causes the following actions on the client side

The client closes the NPC animation and shows an error popup on failure. On success the player has already been teleported to the event map.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD1  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x01  | Packet header - sub packet type identifier |
| 4 | 1 | EnterResult |  | Result |

### EnterResult Enum

Result of the Kanturu enter request.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Failed | Entry failed (generic failure). |
| 1 | Success | The player has been successfully entered into the event. |