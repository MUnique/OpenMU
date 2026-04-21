# C3 26 - ConsumeItemRequest (client gửi)

## Được gửi khi nào

Người chơi yêu cầu 'tiêu thụ' một vật phẩm. Đây có thể là một lọ thuốc phục hồi một số loại thuộc tính hoặc một viên ngọc để nâng cấp vật phẩm mục tiêu.

## Hành động phía server

Máy chủ cố gắng 'tiêu thụ' mục được chỉ định và phản hồi tương ứng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 6 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x26 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte |  | Khe vật phẩm; Chỉ số vùng tồn kho của vật phẩm sẽ được tiêu thụ. |
| 4 | 1 | Byte |  | Khe mục tiêu; Nếu mặt hàng này có tác dụng lên một mặt hàng khác, ví dụ: nâng cấp nó, trường này chứa chỉ mục vị trí kho của mục tiêu. |
| 5 | 1 | FruitUsage |  | Tiêu thụ trái cây; Xác định cách sử dụng trái cây. Chỉ áp dụng nếu vật phẩm là trái cây. |

### Enum FruitUsage
Xác định cách sử dụng trái cây. Chỉ áp dụng nếu vật phẩm là trái cây.

| Value | Name | Description |
|-------|------|-------------|
| 0 | AddPoints | Thêm 1~3 điểm chỉ số cho nhân vật. |
| 1 | RemovePoints | Xóa 1~9 điểm chỉ số khỏi nhân vật. |