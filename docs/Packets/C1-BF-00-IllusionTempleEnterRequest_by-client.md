# C1 BF 00 - IllusionTempleEnterRequest (by client)

## Is sent when

The client has the NPC dialog for the illusion temple opened, and wants to enter the event map.

## Causes the following actions on the server side

The server checks if the player has the required ticket and moves the player to the event map.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBF  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x00  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | MapNumber |
| 5 | 1 | Byte |  | ItemSlot |