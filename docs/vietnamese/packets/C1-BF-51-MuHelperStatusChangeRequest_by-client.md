# C1 BF 51 - MuHelperStatusChangeRequest (client gửi)

## Được gửi khi nào

Khách hàng nhấp vào nút phát hoặc tạm dừng MU Helper.

## Hành động phía server

Máy chủ xác thực xem người dùng có thể sử dụng trình trợ giúp và gửi lại trạng thái hay không.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xBF | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x51 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Boolean |  | Tạm dừngTrạng thái |