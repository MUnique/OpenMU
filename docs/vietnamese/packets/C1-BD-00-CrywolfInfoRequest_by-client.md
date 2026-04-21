# C1 BD 00 - CrywolfInfoRequest (client gửi)

## Được gửi khi nào

Một người chơi bước vào bản đồ crywolf.

## Hành động phía server

Máy chủ trả về dữ liệu về trạng thái của bản đồ crywolf.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xBD | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x00 | Tiêu đề gói - mã định danh loại gói phụ |