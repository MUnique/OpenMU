# C1 F3 05 - CharacterLevelUpdate (server gửi)

## Được gửi khi nào

Sau khi nhân vật lên cấp.

## Hành động phía client

Cập nhật cấp độ (và các số liệu thống kê liên quan khác) trong ứng dụng trò chơi và hiển thị hiệu ứng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 24 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF3 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x05 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 2 | ShortLittleEndian |  | Mức độ |
| 6 | 2 | ShortLittleEndian |  | LevelUpPoints |
| 8 | 2 | ShortLittleEndian |  | Sức khỏe tối đa |
| 10 | 2 | ShortLittleEndian |  | Mana tối đa |
| 12 | 2 | ShortLittleEndian |  | Lá chắn tối đa |
| 14 | 2 | ShortLittleEndian |  | Khả năng tối đa |
| 16 | 2 | ShortLittleEndian |  | FruitPoints |
| 18 | 2 | ShortLittleEndian |  | Điểm trái cây tối đa |
| 20 | 2 | ShortLittleEndian |  | Điểm trái cây tiêu cực |
| 22 | 2 | ShortLittleEndian |  | Điểm trái cây tiêu cực tối đa |