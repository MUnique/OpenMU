# C3 24 - ItemMoveRequestExtended (by client)

## Is sent when

A player requests to move an item within or between his available item storage, such as inventory, vault, trade or chaos machine.

## Causes the following actions on the server side



## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   7   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x24  | Packet header - packet type identifier |
| 3 | 1 | ItemStorageKind |  | FromStorage |
| 4 | 1 | Byte |  | FromSlot |
| 5 | 1 | ItemStorageKind |  | ToStorage |
| 6 | 1 | Byte |  | ToSlot |

### ItemStorageKind Enum

Defines from or to which item storage an item is moved.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Inventory | The inventory storage. |
| 1 | Trade | The trade storage. |
| 2 | Vault | The vault storage. |
| 3 | ChaosMachine | The chaos machine storage. |
| 4 | PlayerShop | The player shop storage. |
| 5 | PetTrainer | The pet trainer dialog storage. |
| 6 | Refinery | The storage of the refinery of the elphis npc. |
| 7 | Smelting | The storage of the smelting dialog of the osbourne npc. |
| 8 | ItemRestore | The storage of the item restore dialog of the jerridon npc. |
| 9 | ChaosCardMaster | The storage of the chaos card master dialog. |
| 10 | CherryBlossomSpirit | The storage of the cherry blossom spirit dialog. |
| 11 | SeedCrafting | The storage of the seed crafting dialog. |
| 12 | SeedSphereCrafting | The storage of the seed sphere crafting dialog. |
| 13 | SeedMountCrafting | The storage of the seed sphere mount dialog. |
| 14 | SeedUnmountCrafting | The storage of the seed sphere unmount dialog. |