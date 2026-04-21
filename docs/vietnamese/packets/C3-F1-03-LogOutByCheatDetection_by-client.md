# C3 F1 03 - LogOutByCheatDetection (client gửi)

## Được gửi khi nào

Khi khách hàng muốn rời khỏi trò chơi theo nhiều cách khác nhau.

## Hành động phía server

Tùy thuộc vào LogOutType, máy chủ trò chơi thực hiện một số kiểm tra và gửi phản hồi lại cho máy khách. Nếu yêu cầu thành công, ứng dụng khách trò chơi sẽ đóng trò chơi, quay lại máy chủ hoặc chọn nhân vật.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 6 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF1 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x03 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Byte | 4 | Type |
| 5 | 1 | Byte |  | Thông số |