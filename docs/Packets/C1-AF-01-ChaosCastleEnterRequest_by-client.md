# C1 AF 01 - ChaosCastleEnterRequest (by client)

## Is sent when

The player requests to enter the chaos castle by using the 'Armor of Guardsman' item.

## Causes the following actions on the server side

The server checks if the player can enter the event and sends a response (Code 0xAF) back to the client. If it was successful, the character gets moved to the event map.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xAF  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x01  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | CastleLevel; The level of the chaos castle. Appears to always be 0. |
| 5 | 1 | Byte |  | TicketItemInventoryIndex; The index of the ticket item in the inventory. |