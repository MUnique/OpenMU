# C1 D2 13 - CashShopEventItemListRequest (by client)

## Is sent when

When the player wants to see through the event item list.

## Causes the following actions on the server side

The server sends a list with event items back.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   8   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x13  | Packet header - sub packet type identifier |
| 4 | 4 | IntegerLittleEndian |  | CategoryIndex |