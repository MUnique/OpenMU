# C1 34 - RepairItemRequest (client gửi)

## Được gửi khi nào

Một người chơi muốn sửa chữa một món đồ trong kho của mình.

## Hành động phía server

Vật phẩm sẽ được sửa chữa nếu người chơi có đủ tiền trong kho. Một phản hồi tương ứng được gửi đi.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x34 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte |  | Khe vật phẩm; Khe chứa vật phẩm tồn kho của vật phẩm mục tiêu. Nếu là 0xFF, người chơi muốn sửa chữa tất cả các vật phẩm - điều này chỉ có thể thực hiện được với một số hộp thoại NPC đã mở. Chỉ có thể sửa chữa khe chứa vật phẩm thú cưng (8) khi mở npc huấn luyện thú cưng. |
| 4 | 1 | Boolean |  | IsSelfRepair; Nếu người chơi sửa chữa nó trên kho đồ của mình thì đó là sự thật. Tuy nhiên, máy chủ không bao giờ nên dựa vào cờ này và tự kiểm tra. |