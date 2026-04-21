# C3 34 - ItemRepair (client gửi)

## Được gửi khi nào

Người chơi muốn sửa chữa một vật phẩm trong kho của mình, bằng chính mình hoặc bằng cách sử dụng NPC.

## Hành động phía server

Nếu mặt hàng bị hư hỏng và có thể sửa chữa được, độ bền của mặt hàng đó sẽ được tối đa hóa và các phản hồi tương ứng sẽ được gửi lại cho khách hàng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x34 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte |  | InventoryItemSlot; Khe chứa đồ của vật phẩm mục tiêu. Nếu là 0xFF, người chơi yêu cầu sửa chữa tất cả vật phẩm với sự trợ giúp của NPC. Nếu là 8 (Khe thú cưng), việc sử dụng NPC huấn luyện thú cưng cũng là điều bắt buộc. |