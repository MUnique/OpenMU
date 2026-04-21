# C1 26 FE - MaximumHealthAndShield (server gửi)

## Được gửi khi nào

Khi sức khỏe tối đa thay đổi, ví dụ: bằng cách thêm điểm chỉ số hoặc thay đổi vật phẩm.

## Hành động phía client

Thanh máu và lá chắn được cập nhật trên giao diện người dùng client trò chơi.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 9 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x26 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0xFE | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 2 | ShortBigEndian |  | Sức khỏe |
| 7 | 2 | ShortBigEndian |  | Khiên |