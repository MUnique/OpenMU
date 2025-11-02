# C1 D2 0B - CashShopStorageItemConsumeRequest (by client)

## Is sent when

The player wants to get or consume an item which is in the cash shop storage.

## Causes the following actions on the server side

The item is applied or added to the inventory.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x0B  | Packet header - sub packet type identifier |
| 4 | 4 | IntegerLittleEndian |  | BaseItemCode |
| 8 | 4 | IntegerLittleEndian |  | MainItemCode |
| 12 | 2 | ShortLittleEndian |  | ItemIndex |
| 14 | 1 | Byte |  | ProductType |