# C3 24 - ItemMoved (server gửi)

## Được gửi khi nào

Khi item trong inventory hoặc vault của người chơi đã được di chuyển.

## Hành động phía client

Client cập nhật vị trí item trong giao diện.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte |  | Packet header - độ dài packet |
| 2 | 1 | Byte | 0x24 | Packet header - packet type identifier |
| 3 | 1 | ItemStorageKind |  | TargetStorageType |
| 4 | 1 | Byte |  | TargetSlot |
| 5 |  | Binary |  | ItemData |

### Enum ItemStorageKind

Định nghĩa storage nguồn/đích khi di chuyển item.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Inventory | Storage inventory. |
| 1 | Trade | Storage trade. |
| 2 | Vault | Storage vault. |
| 3 | ChaosMachine | Storage chaos machine. |
| 4 | PlayerShop | Storage player shop. |
| 5 | PetTrainer | Storage dialog pet trainer. |
| 6 | Refinery | Storage refinery của NPC Elphis. |
| 7 | Smelting | Storage smelting của NPC Osbourne. |
| 8 | ItemRestore | Storage item restore của NPC Jerridon. |
| 9 | ChaosCardMaster | Storage dialog chaos card master. |
| 10 | CherryBlossomSpirit | Storage dialog cherry blossom spirit. |
| 11 | SeedCrafting | Storage dialog seed crafting. |
| 12 | SeedSphereCrafting | Storage dialog seed sphere crafting. |
| 13 | SeedMountCrafting | Storage dialog seed sphere mount. |
| 14 | SeedUnmountCrafting | Storage dialog seed sphere unmount. |
