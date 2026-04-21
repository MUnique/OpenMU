# C1 26 FE - MaximumStatsExtended (server gửi)

## Được gửi khi nào

Khi các chỉ số tối đa, như máu, lá chắn, năng lượng hoặc tốc độ tấn công thay đổi ở phía máy chủ, ví dụ: bằng cách thêm điểm chỉ số hoặc thay đổi vật phẩm.

## Hành động phía client

Các giá trị được cập nhật trên giao diện người dùng máy khách trò chơi.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 20 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x26 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0xFE | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 4 | IntegerLittleEndian |  | Sức khỏe |
| 8 | 4 | IntegerLittleEndian |  | Khiên |
| 12 | 4 | IntegerLittleEndian |  | mana |
| 16 | 4 | IntegerLittleEndian |  | Khả năng |