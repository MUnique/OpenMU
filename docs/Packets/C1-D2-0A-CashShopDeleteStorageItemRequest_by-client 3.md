# C1 D2 0A - CashShopDeleteStorageItemRequest (by client)

## Is sent when

The player wants to delete an item of the cash shop storage.

## Causes the following actions on the server side

The server removes the item from cash shop storage.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   234   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x0A  | Packet header - sub packet type identifier |
| 4 | 4 | IntegerLittleEndian |  | BaseItemCode |
| 8 | 4 | IntegerLittleEndian |  | MainItemCode |
| 12 | 1 | Byte |  | ProductType |