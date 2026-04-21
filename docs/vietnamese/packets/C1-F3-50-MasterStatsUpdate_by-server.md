# C1 F3 50 - MasterStatsUpdate (server gửi)

## Được gửi khi nào

Sau khi vào game với nhân vật đẳng cấp bậc thầy.

## Hành động phía client

Dữ liệu liên quan đến tổng thể có sẵn.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 32 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF3 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x50 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 2 | ShortLittleEndian |  | Bậc thầy |
| 6 | 8 | LongBigEndian |  | Thạc sĩKinh nghiệm |
| 14 | 8 | LongBigEndian |  | Bậc thầyTrải nghiệmCấp độ tiếp theo |
| 22 | 2 | ShortLittleEndian |  | MasterLevelUpPoints |
| 24 | 2 | ShortLittleEndian |  | Sức khỏe tối đa |
| 26 | 2 | ShortLittleEndian |  | Mana tối đa |
| 28 | 2 | ShortLittleEndian |  | Lá chắn tối đa |
| 30 | 2 | ShortLittleEndian |  | Khả năng tối đa |