# C1 AF 01 - ChaosCastleEnterResult (server gửi)

## Được gửi khi nào

Người chơi yêu cầu tham gia mini game lâu đài hỗn loạn bằng cách sử dụng vật phẩm 'Giáp của Vệ binh'.

## Hành động phía client

Trong trường hợp thất bại, nó hiển thị thông báo lỗi tương ứng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xAF | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x01 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | EnterResult |  | Kết quả |

### Enum EnterResult
Xác định kết quả của yêu cầu nhập.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Success | Sự kiện đã được tham gia. |
| 1 | Failed | Việc nhập sự kiện không thành công, ví dụ: do thiếu vé sự kiện hoặc phạm vi cấp độ. |
| 2 | NotOpen | Việc tham gia sự kiện không thành công vì nó chưa được mở. |
| 5 | Full | Tham gia sự kiện không thành công vì sự kiện đã đầy. |
| 7 | NotEnoughMoney | Việc tham gia sự kiện không thành công do người chơi không có đủ tiền để đóng phí vào cửa. |
| 8 | PlayerKillerCantEnter | Việc tham gia sự kiện không thành công do người chơi có trạng thái pk. |