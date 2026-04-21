# C1 C8 - RemoveLetter (server gửi)

## Được gửi khi nào

Sau khi một lá thư đã bị xóa theo yêu cầu của người chơi.

## Hành động phía client

Bức thư được xóa khỏi danh sách thư.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 6 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xC8 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Boolean | true | Yêu cầu thành công |
| 4 | 2 | ShortLittleEndian |  | Thư mục lục |