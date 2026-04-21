# C1 26 FF - CurrentHealthAndShield (server gửi)

## Được gửi khi nào

Định kỳ hoặc nếu lượng máu hoặc lá chắn hiện tại thay đổi ở phía máy chủ, ví dụ: bằng lượt truy cập.

## Hành động phía client

Thanh máu và lá chắn được cập nhật trên giao diện người dùng client trò chơi.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 9 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x26 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0xFF | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 2 | ShortBigEndian |  | Sức khỏe |
| 7 | 2 | ShortBigEndian |  | Khiên |