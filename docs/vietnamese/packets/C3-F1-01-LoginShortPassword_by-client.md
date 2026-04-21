# C3 F1 01 - LoginShortPassword (client gửi)

## Được gửi khi nào

Người chơi cố gắng đăng nhập vào trò chơi.

## Hành động phía server

Máy chủ đang xác thực tên đăng nhập và mật khẩu đã gửi. Nếu đúng, trạng thái của người chơi đang tiến hành đăng nhập.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 50 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF1 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x01 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 10 | Binary |  | Tên người dùng; Tên người dùng, "được mã hóa" bằng Xor3. |
| 14 | 10 | Binary |  | Mật khẩu; Mật khẩu được "mã hóa" bằng Xor3. |
| 24 | 4 | IntegerBigEndian |  | Đếm đánh dấu |
| 28 | 5 | Binary |  | Phiên bản khách hàng |
| 33 | 16 | Binary |  | Khách hàng nối tiếp |