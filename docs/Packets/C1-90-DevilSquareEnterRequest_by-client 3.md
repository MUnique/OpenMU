# C1 90 - DevilSquareEnterRequest (by client)

## Is sent when

The player requests to enter the devil square through the Charon NPC.

## Causes the following actions on the server side

The server checks if the player can enter the event and sends a response (Code 0x90) back to the client. If it was successful, the character gets moved to the event map.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x90  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | SquareLevel; The level of the devil square, minus 1. |
| 4 | 1 | Byte |  | TicketItemInventoryIndex; The index of the ticket item in the inventory. Be aware, that the value is 12 higher than it should be - it makes no sense, but it is what it is... |