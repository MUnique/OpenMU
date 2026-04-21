# C1 F8 0B - GensRankingRequest (client gửi)

## Được gửi khi nào

Ứng dụng khách trò chơi yêu cầu thông tin về thứ hạng thị tộc hiện tại.

## Hành động phía server

Máy chủ trả về thông tin xếp hạng gen hiện tại.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF8 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x0B | Tiêu đề gói - mã định danh loại gói phụ |