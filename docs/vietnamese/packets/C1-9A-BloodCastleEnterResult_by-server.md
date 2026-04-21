# C1 9A - BloodCastleEnterResult (server gửi)

## Được gửi khi nào

Người chơi yêu cầu tham gia mini game lâu đài máu thông qua NPC Tổng lãnh thiên thần.

## Hành động phía client

Trong trường hợp thất bại, nó hiển thị thông báo lỗi tương ứng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x9A | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | EnterResult |  | Kết quả |

### Enum EnterResult
Xác định kết quả của yêu cầu nhập.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Success | Sự kiện đã được tham gia. |
| 1 | Failed | Việc nhập sự kiện không thành công, ví dụ: do thiếu vé sự kiện hoặc phạm vi cấp độ. |
| 2 | NotOpen | Việc tham gia sự kiện không thành công vì nó chưa được mở. |
| 3 | CharacterLevelTooHigh | Nhập sự kiện không thành công vì cấp độ nhân vật quá cao so với cấp độ sự kiện được yêu cầu. |
| 4 | CharacterLevelTooLow | Nhập sự kiện không thành công vì cấp độ nhân vật quá thấp so với cấp độ sự kiện được yêu cầu. |
| 5 | Full | Tham gia sự kiện không thành công vì sự kiện đã đầy. |