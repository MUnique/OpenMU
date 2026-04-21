# C1 F8 03 - GensLeaveRequest (client gửi)

## Được gửi khi nào

Người chơi muốn rời khỏi thị tộc hiện tại.

## Hành động phía server

Máy chủ của người chơi từ gens.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF8 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x03 | Tiêu đề gói - mã định danh loại gói phụ |