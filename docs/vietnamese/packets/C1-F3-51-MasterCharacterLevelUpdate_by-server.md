# C1 F3 51 - MasterCharacterLevelUpdate (server gửi)

## Được gửi khi nào

Sau khi nhân vật chính lên cấp.

## Hành động phía client

Cập nhật cấp độ chính (và các số liệu thống kê liên quan khác) trong ứng dụng khách trò chơi và hiển thị hiệu ứng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 20 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF3 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x51 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 2 | ShortLittleEndian |  | Bậc thầy |
| 6 | 2 | ShortLittleEndian |  | Đạt được MasterPoints |
| 8 | 2 | ShortLittleEndian |  | MasterPoints hiện tại |
| 10 | 2 | ShortLittleEndian |  | Điểm MasterPoints tối đa |
| 12 | 2 | ShortLittleEndian |  | Sức khỏe tối đa |
| 14 | 2 | ShortLittleEndian |  | Mana tối đa |
| 16 | 2 | ShortLittleEndian |  | Lá chắn tối đa |
| 18 | 2 | ShortLittleEndian |  | Khả năng tối đa |