# C3 16 - ExperienceGained (server gửi)

## Được gửi khi nào

Một người chơi đã có được kinh nghiệm.

## Hành động phía client

Kinh nghiệm được thêm vào quầy và thanh kinh nghiệm.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 9 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x16 | Tiêu đề gói - mã định danh loại gói |
| 3 | 2 | ShortBigEndian |  | Id đối tượng bị giết |
| 5 | 2 | ShortBigEndian |  | Đã thêm kinh nghiệm |
| 7 | 2 | ShortBigEndian |  | Sát thương của lần truy cập cuối cùng |