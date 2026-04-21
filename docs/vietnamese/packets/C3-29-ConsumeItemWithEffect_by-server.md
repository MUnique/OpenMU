# C3 29 - ConsumeItemWithEffect (server gửi)

## Được gửi khi nào

Khách hàng yêu cầu tiêu thụ một mặt hàng đặc biệt, ví dụ: một chai rượu Ale.

## Hành động phía client

Người chơi được hiển thị màu đỏ và được tăng tốc độ tấn công.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 6 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x29 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | ConsumedItemType |  | Loại vật phẩm |
| 4 | 2 | ShortLittleEndian |  | Hiệu ứngThời gianTrong giây |

### Enum ConsumedItemType
Xác định một mặt hàng được tiêu thụ.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Ale | Người chơi uống một chai bia, thời gian hiệu ứng thường là 80 giây. |
| 1 | RedemyOfLove | Người chơi nhận được sự cứu chuộc bằng tình yêu, thời gian hiệu lực thường là 90 giây. |
| 77 | PotionOfSoul | Người chơi tiêu thụ một lọ thuốc linh hồn, thời gian hiệu ứng thường là 60 giây. |