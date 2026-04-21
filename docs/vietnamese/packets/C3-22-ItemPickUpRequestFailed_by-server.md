# C3 22 - ItemPickUpRequestFailed (server gửi)

## Được gửi khi nào

Người chơi yêu cầu nhặt một vật phẩm từ mặt đất để thêm vào kho của mình nhưng không thành công.

## Hành động phía client

Tùy thuộc vào lý do, ứng dụng khách trò chơi sẽ hiển thị thông báo.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x22 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | ItemPickUpFailReason |  | Lý do thất bại |

### Enum ItemPickUpFailReason
Xác định các lý do thất bại có thể xảy ra

| Value | Name | Description |
|-------|------|-------------|
| 253 | ItemStacked | Vật phẩm nhặt được được kết hợp thành một vật phẩm hiện có trong kho của người chơi. Một bản cập nhật độ bền riêng biệt sẽ được gửi cho khách hàng. |
| 254 | __MaximumInventoryMoneyReached | Đã đạt đến số tiền tồn kho tối đa nên chưa nhận được tiền. Không nên sử dụng vì nó được sử dụng trong thông báo InventoryMoneyUpdate. |
| 255 | General | Lý do chung chung, không cụ thể. Nó chỉ thất bại. |