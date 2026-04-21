# C3 3F 01 - PlayerShopSetItemPriceResponse (server gửi)

## Được gửi khi nào

Người chơi yêu cầu đặt giá cho một vật phẩm trong cửa hàng của người chơi.

## Hành động phía client

Mặt hàng này có giá trên giao diện người dùng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 6 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x3F | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x01 | Tiêu đề gói - mã định danh loại gói phụ |
| 5 | 1 | Byte |  | Khe hàng tồn kho |
| 4 | 1 | ItemPriceSetResult |  | Kết quả |

### Enum ItemPriceSetResult
Mô tả các kết quả có thể có của việc đặt giá vật phẩm trong cửa hàng người chơi.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Failed | Không thành công, ví dụ: vì tính năng cửa hàng đã bị vô hiệu hóa |
| 1 | Success | Giá đã được đặt thành công |
| 2 | ItemSlotOutOfRange | Không thành công vì ô vật phẩm nằm ngoài phạm vi |
| 3 | ItemNotFound | Không thành công vì không thể tìm thấy mục |
| 4 | PriceNegative | Không thành công vì giá âm |
| 5 | ItemIsBlocked | Không thành công vì mục này bị chặn |
| 6 | CharacterLevelTooLow | Không thành công vì cấp độ nhân vật quá thấp (dưới cấp 6) |