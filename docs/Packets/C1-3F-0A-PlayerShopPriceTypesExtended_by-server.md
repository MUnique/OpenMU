# C1 3F 0A - PlayerShopPriceTypesExtended (by server)

## Is sent when

After entering the game world, and before the player shop of another player is shown, when the server allows prices in player shops which are not paid in zen.

## Causes the following actions on the client side

The client offers the allowed price types, in addition to zen, when the player sets an item price in his player shop. Without this message, only zen prices are possible.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x3F  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x0A  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Count |
| 5 | PlayerShopPriceType.Length * Count | Array of PlayerShopPriceType |  | PriceTypes |

### PlayerShopPriceType Structure

A price type which is allowed for prices in player shops, in addition to zen.

Length: 2 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortLittleEndian |  | PriceItemType; The value of PriceItemType of a price of this type, like in PlayerShopItemExtended: the item (group in the highest 4 bits, number in the remaining ones) with which the price is paid, or 0xFFF0 = WCoin (C), 0xFFF1 = WCoin (P), 0xFFF2 = Goblin Points. |