# C3 3F 09 - PlayerShopSetItemPriceExtended (by client)

## Is sent when

The player wants to set a price of an item which is inside his personal item shop, in one of the currencies which are enabled on the server (see PlayerShopCurrenciesExtended).

## Causes the following actions on the server side

The price is set for the specified item, if the currency is enabled and the price is within the configured limits. Works only if the shop is currently closed. The server responds with a PlayerShopSetItemPriceResponse.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   10   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x3F  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x09  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | ItemSlot |
| 5 | 1 | ShopCurrencyType |  | Currency |
| 6 | 4 | IntegerLittleEndian |  | Price; The price amount in the specified currency. For jewels, it's the number of jewel pieces. |

### ShopCurrencyType Enum

Defines the currency in which the price of an item in a player shop is defined. The value is also the bit index in the currency mask of the PlayerShopCurrenciesExtended message.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Zen | Zen, the money in the inventory. |
| 1 | JewelOfBless | Jewel of Bless (item group 14, number 13). |
| 2 | JewelOfSoul | Jewel of Soul (item group 14, number 14). |
| 3 | JewelOfChaos | Jewel of Chaos (item group 12, number 15). |
| 4 | JewelOfCreation | Jewel of Creation (item group 14, number 22). |
| 5 | JewelOfLife | Jewel of Life (item group 14, number 16). |
| 6 | WCoinC | WCoin (C), an account coin. |
| 7 | WCoinP | WCoin (P), an account coin. |
| 8 | GoblinPoints | Goblin Points, an account coin. |