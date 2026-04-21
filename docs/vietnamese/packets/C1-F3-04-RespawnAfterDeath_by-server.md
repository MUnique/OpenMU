# C1 F3 04 - RespawnAfterDeath (server gửi)

## Được gửi khi nào

Nhân vật hồi sinh sau khi chết.

## Hành động phía client

Nhân vật sẽ hồi sinh với các thuộc tính được chỉ định tại bản đồ đã chỉ định.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 28 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF3 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x04 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Byte |  | Vị tríX |
| 5 | 1 | Byte |  | Vị tríY |
| 6 | 1 | Byte |  | Bản đồSố |
| 7 | 1 | Byte |  | Phương hướng |
| 8 | 2 | ShortLittleEndian |  | Sức khỏe hiện tại |
| 10 | 2 | ShortLittleEndian |  | Mana hiện tại |
| 12 | 2 | ShortLittleEndian |  | CurrentShield |
| 14 | 2 | ShortLittleEndian |  | Khả năng hiện tại |
| 16 | 8 | LongBigEndian |  | Kinh nghiệm |
| 24 | 4 | IntegerLittleEndian |  | Tiền bạc |