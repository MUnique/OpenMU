# C1 9D - LuckyNumberRequest (client gửi)

## Được gửi khi nào

Người chơi yêu cầu đổi mã giảm giá (số may mắn) dài 12 chữ số.

## Hành động phía server

Một phản hồi sẽ được gửi lại cho khách hàng cùng với kết quả. Một mặt hàng có thể được thưởng vào kho.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 18 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x9D | Tiêu đề gói - mã định danh loại gói |
| 3 | 4 | String |  | nối tiếp1 |
| 8 | 4 | String |  | nối tiếp2 |
| 13 | 4 | String |  | nối tiếp3 |