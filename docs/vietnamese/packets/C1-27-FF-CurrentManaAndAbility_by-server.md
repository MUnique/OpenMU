# C1 27 FF - CurrentManaAndAbility (server gửi)

## Được gửi khi nào

Năng lượng hoặc khả năng hiện có đã thay đổi, ví dụ: bằng cách sử dụng một kỹ năng.

## Hành động phía client

Thanh năng lượng và khả năng được cập nhật trên giao diện người dùng ứng dụng trò chơi.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 8 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x27 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0xFF | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 2 | ShortBigEndian |  | mana |
| 6 | 2 | ShortBigEndian |  | Khả năng |