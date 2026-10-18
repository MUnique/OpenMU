# C3 3F 09 - PlayerShopSetItemPriceExtended (by client)

## Is sent when

The player wants to set a price of an item which is inside his personal item shop, in zen or in one of the price types which are allowed by the server (see PlayerShopPriceTypesExtended).

## Causes the following actions on the server side

The price is set for the specified item, if its price type is allowed and the amount is within the configured limits. Works only if the shop is currently closed. The server responds with a PlayerShopSetItemPriceResponse.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   13   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x3F  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x09  | Packet header - sub packet type identifier |
| 4 | 4 | IntegerLittleEndian |  | MoneyPrice; The price in zen, or in account coins when PriceItemType is one of the account coin values. Encoded like the price of PlayerShopItemExtended. |
| 8 | 2 | ShortLittleEndian |  | PriceItemType; The item (group in the highest 4 bits, number in the remaining ones) with which the price is paid, when RequiredItemAmount is greater than 0. Otherwise, 0xFFF0 = WCoin (C), 0xFFF1 = WCoin (P) and 0xFFF2 = Goblin Points define an account coin price; any other value a zen price. Encoded like the price of PlayerShopItemExtended. |
| 10 | 2 | ShortLittleEndian |  | RequiredItemAmount; The number of pieces of the item which is defined by PriceItemType; 0, if the price is not paid with items. Encoded like the price of PlayerShopItemExtended. |
| 12 | 1 | Byte |  | ItemSlot |