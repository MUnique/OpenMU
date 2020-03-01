# C3 3F 01 - PlayerShopSetItemPriceResponse (by server)

## Is sent when

The player requested to set a price for an item of the players shop.

## Causes the following actions on the client side

The item gets a price on the user interface.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x3F  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x01  | Packet header - sub packet type identifier |
| 5 | 1 | Byte |  | InventorySlot |
| 4 | 1 | ItemPriceSetResult |  | Result |

### ItemPriceSetResult Enum

Describes the possible results of setting an item price in a player shop.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Failed | Failed, e.g. because the shop feature is deactivated |
| 1 | Success | The price has been set successfully |
| 2 | ItemSlotOutOfRange | Failed because the item slot was out of range |
| 3 | ItemNotFound | Failed because the item could not be found |
| 4 | PriceNegative | Failed because the price was negative |
| 5 | ItemIsBlocked | Failed because the item is blocked |
| 6 | CharacterLevelTooLow | Failed because the character level is too low (below level 6) |