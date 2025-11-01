# C1 D2 04 - CashShopItemGiftRequest (by client)

## Is sent when

The player wants to send a gift to another player.

## Causes the following actions on the server side

The server buys the item with the credits of the player and sends it as gift to the other player.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   234   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x04  | Packet header - sub packet type identifier |
| 4 | 4 | IntegerLittleEndian |  | PackageMainIndex |
| 8 | 4 | IntegerLittleEndian |  | Category |
| 12 | 4 | IntegerLittleEndian |  | ProductMainIndex |
| 16 | 2 | ShortLittleEndian |  | ItemIndex |
| 18 | 4 | IntegerLittleEndian |  | CoinIndex |
| 22 | 1 | Byte |  | MileageFlag |
| 23 | 11 | String |  | GiftReceiverName |
| 34 | 200 | String |  | GiftText |