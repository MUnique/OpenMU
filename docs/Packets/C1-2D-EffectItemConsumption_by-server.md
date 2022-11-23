# C1 2D - EffectItemConsumption (by server)

## Is sent when

The player requested to consume an item which gives a magic effect.

## Causes the following actions on the client side

The client updates the user interface, it shows the remaining time at the effect icon.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   17   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x2D  | Packet header - packet type identifier |
| 4 | 1 | EffectOrigin |  | Origin |
| 6 | 1 | EffectType |  | Type |
| 8 | 1 | EffectAction |  | Action |
| 12 | 4 | IntegerLittleEndian |  | RemainingSeconds |
| 16 | 1 | Byte |  | MagicEffectNumber |

### EffectOrigin Enum

Defines the origin of the effect.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Undefined | Not defined. |
| 1 | HalloweenAndCherryBlossomEvent | Options of Halloween and Cherry Blossom Event items. |
| 2 | CashShopItem | Options of cash shop items, like Seals. |

### EffectAction Enum

Defines the effect option.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Add | Effect is added. |
| 1 | Remove | Effect is removed. |
| 2 | Replace | Effect is removed, because its getting replaced. |

### EffectType Enum

Defines the kind of effect which was applied.

| Value | Name | Description |
|-------|------|-------------|
| 1 | AttackSpeed | Attack speed increase. |
| 2 | Damage | Damage increase. |
| 3 | Defense | Defense increase. |
| 4 | MaximumHealth | Maximum Health increase. |
| 5 | MaximumMana | Maximum Mana increase. |
| 6 | ExperienceRate | Experience rate increase. |
| 7 | DropRate | Drop rate increase. |
| 8 | Sustenance | Sustenance effect, means no experience is gained during this effect. |
| 9 | Strength | Strength stat increase. |
| 10 | Agility | Agility stat increase. |
| 11 | Vitality | Vitality stat increase. |
| 12 | Energy | Energy stat increase. |
| 13 | Leadership | Leadership stat increase. |
| 14 | PhysicalDamage | Physical damage increase. |
| 15 | WizardryDamage | Wizardry damage increase. |
| 16 | Mobility | Mobility increase. |