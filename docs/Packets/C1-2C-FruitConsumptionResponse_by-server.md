# C1 2C - FruitConsumptionResponse (by server)

## Is sent when

The player requested to consume a fruit.

## Causes the following actions on the client side

The client updates the user interface, by changing the added stat points and used fruit points.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   7   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x2C  | Packet header - packet type identifier |
| 3 | 1 | FruitConsumptionResult |  | Result |
| 4 | 2 | ShortLittleEndian |  | StatPoints |
| 6 | 1 | FruitStatType |  | StatType |

### FruitConsumptionResult Enum

Defines the result of the fruit consumption request.

| Value | Name | Description |
|-------|------|-------------|
| 0 | PlusSuccess | Consumption to add points was successful. |
| 1 | PlusFailed | Consumption to add points failed. |
| 2 | PlusPrevented | Consumption to add points was prevented because some conditions were not correct. |
| 3 | MinusSuccess | Consumption to remove points was successful. |
| 4 | MinusFailed | Consumption to remove points failed. |
| 5 | MinusPrevented | Consumption to remove points was prevented because some conditions were not correct. |
| 6 | MinusSuccessCashShopFruit | Consumption to remove points was successful, removed by a fruit acquired through the cash shop. |
| 16 | PreventedByEquippedItems | Consumption was prevented because an item was equipped. |
| 33 | PlusPreventedByMaximum | Consumption to add points was prevented because the maximum amount of points have been added. |
| 37 | MinusPreventedByMaximum | Consumption to remove points was prevented because the maximum amount of points have been removed. |
| 38 | MinusPreventedByDefault | Consumption to remove points was prevented because the base amount of stat points of the character class cannot be undercut. |

### FruitStatType Enum

Defines the type of stat which the fruit modifies.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Energy | Fruit which modifies the energy stat. |
| 1 | Vitality | Fruit which modifies the vitality stat. |
| 2 | Agility | Fruit which modifies the agility stat. |
| 3 | Strength | Fruit which modifies the strength stat. |
| 4 | Leadership | Fruit which modifies the leadership stat. |