# C1 3C - TradeButtonStateChange (client gửi)

## Được gửi khi nào

Người chơi nhấn nút giao dịch.

## Hành động phía server

Sự thay đổi trạng thái được chuyển tiếp đến đối tác thương mại. Nếu cả hai người chơi nhấn nút giao dịch cùng lúc, máy chủ sẽ cố gắng hoàn thành giao dịch bằng cách trao đổi vật phẩm và tiền.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x3C | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | TradeButtonState |  | Bang mới |

### Enum TradeButtonState
Trạng thái của nút giao dịch.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Unchecked | Nút giao dịch không được nhấn. Điều đó có nghĩa là giao dịch chưa được người giao dịch chấp nhận. |
| 1 | Checked | Nút giao dịch được nhấn. Nó có nghĩa là giao dịch được người giao dịch chấp nhận. |
| 2 | Red | Trạng thái này chỉ được gửi tới client. Sau vài giây, máy khách sẽ trở lại trạng thái bình thường. Không được chọn. |