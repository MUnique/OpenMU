# C1 86 - ItemCraftingResult (by server)

## Is sent when

After the player requested to execute an item crafting, e.g. at the chaos machine.

## Causes the following actions on the client side

The game client updates the UI to show the resulting item.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x86  | Packet header - packet type identifier |
| 3 | 1 | CraftingResult |  | Result |
| 4 |  | Binary |  | ItemData |

### CraftingResult Enum

Defines the crafting result.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Failed | The crafting failed. |
| 1 | Success | The crafting succeeded. |
| 2 | NotEnoughMoney | The crafting wasn't executed because of missing money. |
| 3 | TooManyItems | The crafting wasn't executed because of too many items. |
| 4 | CharacterLevelTooLow | The crafting wasn't executed because the character level is too low. |
| 6 | LackingMixItems | The crafting wasn't executed because of missing items. |
| 7 | IncorrectMixItems | The crafting wasn't executed because of incorrect items. |
| 8 | InvalidItemLevel | The crafting wasn't executed because of an invalid item level. |
| 9 | CharacterClassTooLow | The crafting wasn't executed because the character class is too low. |
| 10 | IncorrectBloodCastleItems | The blood castle ticket crafting wasn't executed because the BloodCastle items are not correct. |
| 11 | NotEnoughMoneyForBloodCastle | The crafting wasn't executed because the player has not enough money for the blood castle ticket crafting. |