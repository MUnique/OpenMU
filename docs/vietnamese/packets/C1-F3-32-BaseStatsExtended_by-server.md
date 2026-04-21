# C1 F3 32 - BaseStatsExtended (server gửi)

## Được gửi khi nào

Đặt chỉ số cơ bản của một nhân vật, ví dụ: đặt lệnh thống kê hoặc sau khi thiết lập lại.

## Hành động phía client

Các giá trị được cập nhật trên giao diện người dùng máy khách trò chơi.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 24 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF3 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x32 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 4 | IntegerLittleEndian |  | Sức mạnh |
| 8 | 4 | IntegerLittleEndian |  | Nhanh nhẹn |
| 12 | 4 | IntegerLittleEndian |  | Sức sống |
| 16 | 4 | IntegerLittleEndian |  | Năng lượng |
| 20 | 4 | IntegerLittleEndian |  | Yêu cầu |