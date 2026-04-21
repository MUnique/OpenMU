# C1 3A - SetTradeMoney (client gửi)

## Được gửi khi nào

Người chơi yêu cầu đặt một số tiền trong giao dịch.

## Hành động phía server

Nó được lấy từ số tiền sẵn có của hàng tồn kho. Nếu số tiền mới thấp hơn số tiền đã đặt trước đó, số tiền đó sẽ được thêm trở lại vào kho. Đối tác thương mại được thông báo về bất kỳ thay đổi nào.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 8 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x3A | Tiêu đề gói - mã định danh loại gói |
| 4 | 4 | IntegerLittleEndian |  | Số lượng |