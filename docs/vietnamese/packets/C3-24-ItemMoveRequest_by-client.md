# C3 24 - ItemMoveRequest (client gửi)

## Được gửi khi nào

Khi người chơi yêu cầu di chuyển item trong cùng storage hoặc giữa các storage,
ví dụ inventory, vault, trade hoặc chaos machine.

## Hành động phía server

Server xác thực thao tác và xử lý di chuyển item tương ứng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 19 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0x24 | Packet header - packet type identifier |
| 3 | 1 | ItemStorageKind |  | FromStorage |
| 4 | 1 | Byte |  | FromSlot |
| 5 | 12 | Binary |  | ItemData |
| 17 | 1 | ItemStorageKind |  | ToStorage |
| 18 | 1 | Byte |  | ToSlot |

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
