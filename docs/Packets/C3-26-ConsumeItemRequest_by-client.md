# C3 26 - ConsumeItemRequest (by client)

## Is sent when

A player requests to 'consume' an item. This can be a potion which recovers some kind of attribute, or a jewel to upgrade a target item.

## Causes the following actions on the server side

The server tries to 'consume' the specified item and responses accordingly.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x26  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | ItemSlot; The inventory slot index of the item which should be consumed. |
| 4 | 1 | Byte |  | TargetSlot; If the item has an effect on another item, e.g. upgrading it, this field contains the inventory slot index of the target item. |
| 5 | 1 | FruitUsage |  | FruitConsumption; Defines how the fruit is used. Only applies, if the the item is a fruit. |

### FruitUsage Enum

Defines how the fruit is used. Only applies, if the the item is a fruit.

| Value | Name | Description |
|-------|------|-------------|
| 0 | AddPoints | Adds 1~3 stat points to the character. |
| 1 | RemovePoints | Removes 1~9 stat points from the character. |