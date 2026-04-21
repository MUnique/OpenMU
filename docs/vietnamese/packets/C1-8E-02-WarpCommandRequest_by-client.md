# C1 8E 02 - WarpCommandRequest (client gửi)

## Được gửi khi nào

Người chơi được chọn để làm cong bằng cách chọn một mục trong danh sách làm cong (được định cấu hình trong tệp ứng dụng khách của trò chơi).

## Hành động phía server

Nếu người chơi có đủ tiền và được phép vào bản đồ, nó sẽ được chuyển đến đó.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 10 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x8E | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x02 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 4 | IntegerLittleEndian |  | CommandKey; Khóa lệnh, được tạo bằng thuật toán 'bí mật'. Không được xem xét trong OpenMU. |
| 8 | 2 | ShortLittleEndian |  | WarpInfoIndex; Chỉ mục của mục trong danh sách dọc. |