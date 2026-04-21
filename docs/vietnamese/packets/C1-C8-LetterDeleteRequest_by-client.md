# C1 C8 - LetterDeleteRequest (client gửi)

## Được gửi khi nào

Một người chơi yêu cầu xóa một chữ cái.

## Hành động phía server

Bức thư đang bị xóa.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 6 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xC8 | Tiêu đề gói - mã định danh loại gói |
| 4 | 2 | ShortLittleEndian |  | Thư mục lục |