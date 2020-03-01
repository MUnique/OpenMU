# C1 BC - LahapJewelMixRequest (by client)

## Is sent when

When a player has the Lahap npc dialog open and wants to combine or disband jewel stacks.

## Causes the following actions on the server side

If successful, the inventory is updated and the game client gets corresponding responses.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBC  | Packet header - packet type identifier |
| 3 | 1 | MixType |  | Operation |
| 4 | 1 | ItemType |  | Item |
| 5 | 1 | StackSize |  | MixingStackSize |
| 6 | 1 | Byte |  | UnmixingSourceSlot |

### MixType Enum

Describes what kind of operation is requested.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Mix | The player wants to mix single jewels into a bundle. |
| 1 | Unmix | The player wants to disband a bundle into single jewels. |

### StackSize Enum

Describes the size of a mixed stack.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Ten | A stack of 10 items. |
| 1 | Twenty | A stack of 20 items. |
| 2 | Thirty | A stack of 30 items. |

### ItemType Enum

Describes the type of item which is mixed or unmixed.

| Value | Name | Description |
|-------|------|-------------|
| 0 | JewelOfBless | Jewel of Bless. |
| 1 | JewelOfSoul | Jewel of Soul. |
| 2 | JewelOfLife | Jewel of Life. |
| 3 | JewelOfCreation | Jewel of Creation. |
| 4 | JewelOfGuardian | Jewel of Guardian. |
| 5 | Gemstone | The gemstone. |
| 6 | JewelOfHarmony | Jewel of Harmony. |
| 7 | JewelOfChaos | Jewel of Chaos. |
| 8 | LowerRefineStone | Lower refine stone. |
| 9 | HigherRefineStone | Higher refine stone. |