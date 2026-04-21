# C1 81 - VaultMoveMoneyRequest (client gửi)

## Được gửi khi nào

Người chơi muốn chuyển tiền từ hoặc đến kho lưu trữ.

## Hành động phía server

Tiền sẽ được chuyển đi, nếu có thể.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 8 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x81 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | VaultMoneyMoveDirection |  | Phương hướng |
| 4 | 4 | IntegerLittleEndian |  | Số lượng |

### Enum VaultMoneyMoveDirection
Xác định hướng di chuyển của tiền giữa hàng tồn kho và kho tiền.

| Value | Name | Description |
|-------|------|-------------|
| 0 | InventoryToVault | Tiền được chuyển từ kho đến kho tiền. |
| 1 | VaultToInventory | Tiền được chuyển từ kho tiền vào kho. |