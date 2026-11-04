# C1 D2 03 - CashShopItemBuyRequest (by client)

## Is sent when

The player wants to buy an item in the cash shop.

## Causes the following actions on the server side

The item is bought and added to the cash shop item storage of the player.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   23   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x03  | Packet header - sub packet type identifier |
| 4 | 4 | IntegerLittleEndian |  | PackageMainIndex |
| 8 | 4 | IntegerLittleEndian |  | Category |
| 12 | 4 | IntegerLittleEndian |  | ProductMainIndex |
| 16 | 2 | ShortLittleEndian |  | ItemIndex |
| 18 | 4 | IntegerLittleEndian |  | CoinIndex |
| 22 | 1 | Byte |  | MileageFlag |