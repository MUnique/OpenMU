# C1 3F 06 - PlayerShopBuyResult (by server)

## Is sent when

After the player requested to buy an item of a shop of another player.

## Causes the following actions on the client side

The result is shown to the player. If successful, the item is added to the inventory.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   21   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x3F  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x06  | Packet header - sub packet type identifier |
| 4 | 1 | ResultKind |  | Result |
| 5 | 2 | ShortBigEndian |  | SellerId |
| 7 | 13 | Binary |  | ItemData |
| 20 | 1 | Byte |  | ItemSlot |

### ResultKind Enum

The kind of result.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Undefined | Undefined result. |
| 1 | Success | The item has been bought successfully. |
| 2 | NotAvailable | The seller is not available. |
| 3 | ShopNotOpened | The requested player has no open shop. |
| 4 | InTransaction | The requested player is already in a transaction with another player. |
| 5 | InvalidShopSlot | The requested item slot is invalid. |
| 6 | NameMismatchOrPriceMissing | The requested player with the specified id has a different name or price is missing. |
| 7 | LackOfMoney | The player has not enough money to buy the item from the seller. |
| 8 | MoneyOverflowOrNotEnoughSpace | The selling player cannot sell the item, because the sale would overflow his money amount in the inventory. Another possibility is that the inventory of the buyer cannot take the item. |
| 9 | ItemBlock | The requested player has item block active. |