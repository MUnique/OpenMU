# C1 F3 05 - CharacterLevelUpdateExtended (server gửi)

## Được gửi khi nào

Sau khi nhân vật lên cấp.

## Hành động phía client

Cập nhật cấp độ (và các số liệu thống kê liên quan khác) trong ứng dụng trò chơi và hiển thị hiệu ứng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 32 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF3 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x05 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 2 | ShortLittleEndian |  | Mức độ |
| 6 | 2 | ShortLittleEndian |  | LevelUpPoints |
| 8 | 4 | IntegerLittleEndian |  | Sức khỏe tối đa |
| 12 | 4 | IntegerLittleEndian |  | Mana tối đa |
| 16 | 4 | IntegerLittleEndian |  | Lá chắn tối đa |
| 20 | 4 | IntegerLittleEndian |  | Khả năng tối đa |
| 24 | 2 | ShortLittleEndian |  | FruitPoints |
| 26 | 2 | ShortLittleEndian |  | Điểm trái cây tối đa |
| 28 | 2 | ShortLittleEndian |  | Điểm trái cây tiêu cực |
| 30 | 2 | ShortLittleEndian |  | Điểm trái cây tiêu cực tối đa |