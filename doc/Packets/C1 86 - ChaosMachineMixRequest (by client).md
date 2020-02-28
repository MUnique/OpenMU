# C1 86 - ChaosMachineMixRequest (by client)

## Is sent when

The player has the dialog of the chaos machine open and decided to mix (craft) the items which he put into the chaos machine dialog.

## Causes the following actions on the server side

Based on the type of mix and it's corresponding success rate, the mix succeeds or fails. The client gets a corresponding response with the created, changed or lost items.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x86  | Packet header - packet type identifier |
| 2 | 1 | ChaosMachineMixType |  | MixType; The identifier which tells the server which kind of mix should be executed. |

### ChaosMachineMixType Enum

This enum describes the possible chaos machine mix type identifiers.

| Value | Name | Description |
|-------|------|-------------|
| 1 | ChaosWeapon | Mixes the items to a chaos weapon. |
| 3 | UpgradeItemLevelTo10 | Upgrades an item to level 10. |
| 4 | UpgradeItemLevelTo11 | Upgrades an item to level 11. |
| 22 | UpgradeItemLevelTo12 | Upgrades an item to level 12. |
| 23 | UpgradeItemLevelTo13 | Upgrades an item to level 13. |
| 49 | UpgradeItemLevelTo14 | Upgrades an item to level 14. |
| 50 | UpgradeItemLevelTo15 | Upgrades an item to level 15. |
| 6 | FruitCreation | Mixes the items to a fruit. |
| 41 | GemstoneRefinery | Refines a gemstone to a jewel of harmony. |
| 15 | PotionOfBless | Refines a Jewel Of Bless to a stack of potions of bless. |
| 16 | PotionOfSoul | Refines a Jewel Of Soul to a stack of potions of bless. |