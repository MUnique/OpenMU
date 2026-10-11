# C1 3F 0A - PlayerShopCurrenciesExtended (by server)

## Is sent when

After entering the game world, and before the player shop of the own player or of another player is shown.

## Causes the following actions on the client side

The client offers only the enabled currencies when the player sets an item price in his player shop.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   8   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x3F  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x0A  | Packet header - sub packet type identifier |
| 4 | 4 | IntegerLittleEndian |  | CurrencyMask; The bit mask of the currencies which are enabled for prices in player shops. Bit N corresponds to the ShopCurrencyType with value N, e.g. bit 0 is Zen. A value of 0 means that the feature is disabled and only Zen prices with PlayerShopSetItemPrice are supported. |