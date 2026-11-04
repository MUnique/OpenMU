# C1 BF 0E - DoppelgangerEnterRequest (by client)

## Is sent when

The player wants to enter the doppelganger event.

## Causes the following actions on the server side

The server checks the event ticket and moves the player to the event map.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBF  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x0E  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | TicketItemSlot |