# C3 29 - ConsumeItemWithEffect (by server)

## Is sent when

The client requested to consume a special item, e.g. a bottle of Ale.

## Causes the following actions on the client side

The player is shown in a red color and has increased attack speed.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x29  | Packet header - packet type identifier |
| 3 | 1 | ConsumedItemType |  | ItemType |
| 4 | 2 | ShortLittleEndian |  | EffectTimeInSeconds |

### ConsumedItemType Enum

Defines a consumed item.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Ale | The player consumes a bottle of ale, usually 80 seconds effect time. |
| 1 | RedemyOfLove | The player consumes a redemy of love, usually 90 seconds effect time. |
| 77 | PotionOfSoul | The player consumes a potion of soul, usually 60 seconds effect time. |