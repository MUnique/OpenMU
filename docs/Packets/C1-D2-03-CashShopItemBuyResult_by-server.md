# C1 D2 03 - CashShopItemBuyResult (by server)

## Is sent when

The player requested to buy an item in the cash shop (CashShopItemBuyRequest).

## Causes the following actions on the client side

The client shows a message with the result. If the item was bought, it requests the point information and the first storage page again.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   9   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x03  | Packet header - sub packet type identifier |
| 4 | 1 | CashShopBuyResult |  | Result |
| 5 | 4 | IntegerLittleEndian |  | LeftCount; The number of items which are left for sale, if the number is limited. It's not used by the client. |

### CashShopBuyResult Enum

The result of a request to buy an item in the cash shop.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Success | The item was bought and added to the storage. |
| 1 | NotEnoughCoins | The player doesn't have enough WCoin or Goblin Points. |
| 2 | StorageFull | The storage of the player is full. |
| 3 | SoldOut | The item is sold out. |
| 4 | NotAvailableCurrently | The item is currently not available. |
| 5 | NoLongerAvailable | The item is no longer available. |
| 6 | CannotBeBought | The item can't be bought. |
| 7 | EventItemCannotBeBought | Event items can't be bought. |
| 8 | EventItemLimitExceeded | The maximum number of purchases of the event item is exceeded. |
| 9 | WrongCoinType | The selected coin type is not the one of the item. |
| 254 | DatabaseAccessFailed | The access to the database failed. |
| 255 | DatabaseError | A database error occurred. |