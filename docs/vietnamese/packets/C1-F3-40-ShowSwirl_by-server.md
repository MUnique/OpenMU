# C1 F3 40 - ShowSwirl (server gửi)

## Được gửi khi nào

Ví dụ. khi các vật phẩm sự kiện bị rơi xuống sàn.

## Hành động phía client

Máy khách hiển thị hiệu ứng xoáy tại đối tượng được chỉ định.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 7 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF3 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x40 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Byte | 58 | Loại hiệu ứng |
| 5 | 2 | ShortBigEndian |  | Id đối tượng mục tiêu |