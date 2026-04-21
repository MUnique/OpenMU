# C1 BF 51 - MuHelperStatusUpdate (server gửi)

## Được gửi khi nào

Máy chủ xác thực hoặc thay đổi trạng thái của MU Helper.

## Hành động phía client

Khách hàng chuyển đổi trạng thái Người trợ giúp MU.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 16 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xBF | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x51 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Boolean |  | Tiêu thụTiền; Cờ, nếu tiền nên được tiêu thụ. Nếu đây là 'true' thì việc đặt PauseStatus thành 'false' sẽ không khiến trình trợ giúp khởi động. |
| 8 | 4 | IntegerLittleEndian |  | Tiền bạc |
| 12 | 1 | Boolean |  | Trạng thái tạm dừng; Trạng thái tạm dừng. Giá trị 'true' luôn có tác dụng ngăn chặn người trợ giúp. Tuy nhiên, nó chỉ có thể được bắt đầu khi ConsumeMoney được đặt thành 'false'. |