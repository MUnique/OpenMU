# C1 18 - ObjectAnimation (server gửi)

## Được gửi khi nào

Một đối tượng thực hiện một hình ảnh động.

## Hành động phía client

Hoạt ảnh được hiển thị cho đối tượng được chỉ định.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 9 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x18 | Tiêu đề gói - mã định danh loại gói |
| 3 | 2 | ShortBigEndian |  | ID đối tượng |
| 5 | 1 | Byte |  | Phương hướng |
| 6 | 1 | Byte |  | Hoạt hình |
| 7 | 2 | ShortBigEndian |  | Id mục tiêu |