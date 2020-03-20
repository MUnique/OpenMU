# C3 3F 01 - PlayerShopSetItemPrice (by client)

## Is sent when

The player wants to set a price of an item which is inside his personal item shop.

## Causes the following actions on the server side

The price is set for the specified item. Works only if the shop is currently closed.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x3F  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x01  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | ItemSlot |
| 5 | 4 | IntegerLittleEndian |  | Price |