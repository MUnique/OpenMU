# C1 BF 0D - LuckyCoinExchangeRequest (client gửi)

## Được gửi khi nào

Người chơi mở hộp thoại đồng xu may mắn và yêu cầu trao đổi số lượng xu đã đăng ký được chỉ định.

## Hành động phía server

Máy chủ thêm một vật phẩm vào kho của nhân vật và gửi phản hồi kèm theo mã kết quả.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 8 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xBF | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x0D | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 4 | IntegerLittleEndian |  | Đếm tiền xu |