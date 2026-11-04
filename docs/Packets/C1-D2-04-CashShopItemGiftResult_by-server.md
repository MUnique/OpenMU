# C1 D2 04 - CashShopItemGiftResult (by server)

## Is sent when

The player requested to send an item of the cash shop as gift (CashShopItemGiftRequest).

## Causes the following actions on the client side

The client shows a message with the result. If the gift was sent, it requests the point information again.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   17   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x04  | Packet header - sub packet type identifier |
| 4 | 1 | CashShopGiftResult |  | Result |
| 5 | 4 | IntegerLittleEndian |  | LeftCount; The number of items which are left for sale, if the number is limited. It's not used by the client. |
| 9 | 8 | Double |  | LimitedCash; It's not used by the client. |

### CashShopGiftResult Enum

The result of a request to send an item of the cash shop as gift.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Success | The gift was added to the gift storage of the recipient. |
| 1 | NotEnoughCoins | The player doesn't have enough WCoin or Goblin Points. |
| 2 | RecipientStorageFull | The storage of the recipient is full. |
| 3 | RecipientNotFound | The recipient wasn't found. |
| 4 | SoldOut | The item is sold out. |
| 5 | NoLongerAvailable | The item is no longer available. |
| 6 | NoLongerAvailable2 | The item is no longer available. The client shows it as error. |
| 7 | CannotBeGifted | The item can't be sent as gift. |
| 8 | EventItemCannotBeGifted | The event item can't be sent as gift. |
| 9 | EventItemGiftLimitExceeded | The maximum number of gifts of the event item is exceeded. |
| 10 | WrongCoinType | The selected coin type is not the one of the item. |
| 20 | IdDoesNotExist | The ID doesn't exist. |
| 254 | DatabaseAccessFailed | The access to the database failed. |
| 255 | DatabaseError | A database error occurred. |