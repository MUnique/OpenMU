# C1 F3 51 - MasterCharacterLevelUpdateExtended (server gửi)

## Được gửi khi nào

Sau khi nhân vật chính lên cấp.

## Hành động phía client

Cập nhật cấp độ chính (và các số liệu thống kê liên quan khác) trong ứng dụng khách trò chơi và hiển thị hiệu ứng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 28 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF3 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x51 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 2 | ShortLittleEndian |  | Bậc thầy |
| 6 | 2 | ShortLittleEndian |  | Đạt được MasterPoints |
| 8 | 2 | ShortLittleEndian |  | MasterPoints hiện tại |
| 10 | 2 | ShortLittleEndian |  | Điểm MasterPoints tối đa |
| 12 | 4 | IntegerLittleEndian |  | Sức khỏe tối đa |
| 16 | 4 | IntegerLittleEndian |  | Mana tối đa |
| 20 | 4 | IntegerLittleEndian |  | Lá chắn tối đa |
| 24 | 4 | IntegerLittleEndian |  | Khả năng tối đa |