# C1 2C - FruitConsumptionResponse (server gửi)

## Được gửi khi nào

Người chơi yêu cầu ăn một loại trái cây.

## Hành động phía client

Máy khách cập nhật giao diện người dùng bằng cách thay đổi điểm chỉ số được thêm vào và điểm trái cây đã sử dụng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 7 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x2C | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | FruitConsumptionResult |  | Kết quả |
| 4 | 2 | ShortLittleEndian |  | StatPoints |
| 6 | 1 | FruitStatType |  | Loại trạng thái |

### Enum FruitConsumptionResult
Xác định kết quả của yêu cầu tiêu thụ trái cây.

| Value | Name | Description |
|-------|------|-------------|
| 0 | PlusSuccess | Tiêu thụ để thêm điểm đã thành công. |
| 1 | PlusFailed | Tiêu thụ để thêm điểm không thành công. |
| 2 | PlusPrevented | Việc tiêu thụ để thêm điểm đã bị ngăn chặn vì một số điều kiện không chính xác. |
| 3 | MinusSuccess | Tiêu thụ để loại bỏ điểm đã thành công. |
| 4 | MinusFailed | Tiêu thụ để loại bỏ điểm không thành công. |
| 5 | MinusPrevented | Việc tiêu thụ để loại bỏ điểm đã bị ngăn chặn do một số điều kiện không chính xác. |
| 6 | MinusSuccessCashShopFruit | Việc tiêu thụ để loại bỏ điểm đã thành công, loại bỏ bằng một loại trái cây có được thông qua cửa hàng tiền mặt. |
| 16 | PreventedByEquippedItems | Việc tiêu thụ bị ngăn chặn vì một vật phẩm đã được trang bị. |
| 33 | PlusPreventedByMaximum | Việc tiêu thụ để thêm điểm đã bị ngăn chặn vì số lượng điểm tối đa đã được thêm vào. |
| 37 | MinusPreventedByMaximum | Việc tiêu thụ để loại bỏ điểm đã bị ngăn chặn vì số lượng điểm tối đa đã bị loại bỏ. |
| 38 | MinusPreventedByDefault | Việc tiêu thụ để loại bỏ điểm đã bị ngăn chặn vì không thể cắt giảm lượng điểm chỉ số cơ bản của lớp nhân vật. |

### Enum FruitStatType
Xác định loại chỉ số mà trái cây sửa đổi.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Energy | Trái cây làm thay đổi chỉ số năng lượng. |
| 1 | Vitality | Trái cây điều chỉnh chỉ số sức sống. |
| 2 | Agility | Trái cây điều chỉnh chỉ số nhanh nhẹn. |
| 3 | Strength | Trái cây điều chỉnh chỉ số sức mạnh. |
| 4 | Leadership | Trái cây sửa đổi chỉ số lãnh đạo. |