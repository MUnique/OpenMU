# C2 3F 05 - PlayerShopItemListExtended (by server)

## Is sent when

After the player requested to open a shop of another player.

## Causes the following actions on the client side

The player shop dialog is shown with the provided item data.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0x3F  | Packet header - packet type identifier |
| 4 | 1 |    Byte   | 0x05  | Packet header - sub packet type identifier |
| 4 | 1 | ActionKind |  | Action |
| 5 | 1 | Boolean | true | Success |
| 6 | 2 | ShortBigEndian |  | PlayerId |
| 8 | 10 | String |  | PlayerName |
| 18 | 36 | String |  | ShopName |
| 54 | 1 | Byte |  | ItemCount |
| 55 | PlayerShopItemExtended.Length * ItemCount | Array of PlayerShopItemExtended |  | Items |

### PlayerShopItemExtended Structure

Data of an item in a player shop, which allows for dynamic item sizes and trades for specific kind of items (e.g. jewels), too.

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 4 | IntegerLittleEndian |  | MoneyPrice; The price in zen, or in account coins when PriceItemType is one of the account coin values (see there). It's 0 when the price is defined in items. |
| 4 | 2 | ShortLittleEndian |  | PriceItemType; When RequiredItemAmount is greater than 0, it contains the item group in the highest 4 bits, and the item number in the remaining ones, of the item (e.g. a jewel) which is required to buy the item. When RequiredItemAmount is 0, the following reserved values define an account coin as currency, with the amount in MoneyPrice: 0xFFF0 = WCoin (C), 0xFFF1 = WCoin (P), 0xFFF2 = Goblin Points. Any other value means that MoneyPrice is the price in zen. |
| 6 | 2 | ShortLittleEndian |  | RequiredItemAmount; The number of pieces of the item which is defined by PriceItemType, which is required to buy the item. 0, if the price is not defined in items. |
| 8 | 1 | Byte |  | ItemSlot |
| 9 |  | Binary |  | ItemData |

### ActionKind Enum

The kind of action which led to the list message.

| Value | Name | Description |
|-------|------|-------------|
| 5 | ByRequest | The list was requested. |
| 19 | UpdateAfterItemChange | The list was changed, e.g. because an item was sold. |