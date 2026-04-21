# C1 3C - TradeButtonStateChanged (server gửi)

## Được gửi khi nào

Sau khi đối tác thương mại chọn hoặc bỏ chọn nút chấp nhận giao dịch.

## Hành động phía client

Ứng dụng khách trò chơi sẽ cập nhật trạng thái nút giao dịch tương ứng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x3C | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | TradeButtonState |  | Tình trạng |

### Enum TradeButtonState
Xác định trạng thái của nút giao dịch.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Unchecked | Nút giao dịch không được nhấn. Điều đó có nghĩa là giao dịch chưa được người giao dịch chấp nhận. |
| 1 | Checked | Nút giao dịch được nhấn. Nó có nghĩa là giao dịch được người giao dịch chấp nhận. |
| 2 | Red | Trạng thái này chỉ được gửi tới client. Sau vài giây, máy khách sẽ trở lại trạng thái bình thường. Không được chọn. |