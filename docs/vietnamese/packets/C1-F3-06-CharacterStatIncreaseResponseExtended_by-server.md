# C1 F3 06 - CharacterStatIncreaseResponseExtended (server gửi)

## Được gửi khi nào

Sau khi máy chủ xử lý gói yêu cầu tăng chỉ số ký tự.

## Hành động phía client

Nếu thành công, hãy thêm một điểm vào loại chỉ số được yêu cầu.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 24 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF3 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x06 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | CharacterStatAttribute |  | Thuộc tính |
| 6 | 2 | ShortLittleEndian |  | Số tiền đã thêm |
| 8 | 4 | IntegerLittleEndian |  | Đã cập nhậtSức khỏe tối đa |
| 12 | 4 | IntegerLittleEndian |  | Đã cập nhậtMana tối đa |
| 16 | 4 | IntegerLittleEndian |  | Đã cập nhậtMaximumShield |
| 20 | 4 | IntegerLittleEndian |  | Đã cập nhậtKhả năng tối đa |

### Enum CharacterStatAttribute
Xác định loại thuộc tính stat ký tự.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Strength | Thuộc tính sức mạnh. |
| 1 | Agility | Thuộc tính nhanh nhẹn. |
| 2 | Vitality | Thuộc tính sức sống. |
| 3 | Energy | Thuộc tính năng lượng. |
| 4 | Leadership | Thuộc tính lãnh đạo. |