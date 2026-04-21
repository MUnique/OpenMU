# C3 24 - ItemMoveRequestExtended (client gửi)

## Được gửi khi nào

Người chơi yêu cầu di chuyển một vật phẩm trong hoặc giữa kho vật phẩm có sẵn của mình, chẳng hạn như kho, kho tiền, máy trao đổi hoặc máy hỗn loạn.

## Hành động phía server



## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 7 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x24 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | ItemStorageKind |  | TừStorage |
| 4 | 1 | Byte |  | TừSlot |
| 5 | 1 | ItemStorageKind |  | Lưu trữ |
| 6 | 1 | Byte |  | tới khe cắm |

### Enum ItemStorageKind
Xác định từ hoặc đến nơi lưu trữ mục mà một mục được di chuyển.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Inventory | Việc lưu trữ hàng tồn kho. |
| 1 | Trade | Kho lưu trữ thương mại. |
| 2 | Vault | Kho lưu trữ. |
| 3 | ChaosMachine | Kho lưu trữ máy hỗn loạn. |
| 4 | PlayerShop | Cửa hàng lưu trữ của người chơi. |
| 5 | PetTrainer | Hộp thoại lưu trữ của người huấn luyện thú cưng. |
| 6 | Refinery | Kho lưu trữ của nhà máy lọc dầu của elphis npc. |
| 7 | Smelting | Việc lưu trữ hộp thoại nấu chảy của NPC Osbourne. |
| 8 | ItemRestore | Việc lưu trữ hộp thoại khôi phục vật phẩm của jerridon npc. |
| 9 | ChaosCardMaster | Việc lưu trữ hộp thoại chính của thẻ hỗn loạn. |
| 10 | CherryBlossomSpirit | Nơi lưu trữ hộp thoại tinh thần hoa anh đào. |
| 11 | SeedCrafting | Việc lưu trữ hộp thoại chế tạo hạt giống. |
| 12 | SeedSphereCrafting | Việc lưu trữ hộp thoại chế tạo quả cầu hạt giống. |
| 13 | SeedMountCrafting | Việc lưu trữ hộp thoại gắn kết quả cầu hạt giống. |
| 14 | SeedUnmountCrafting | Việc lưu trữ hộp thoại ngắt kết nối quả cầu hạt giống. |