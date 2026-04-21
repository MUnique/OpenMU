# C1 B2 00 - CastleSiegeStatusRequest (client gửi)

## Được gửi khi nào

Người chơi mở NPC vây hãm lâu đài và yêu cầu thông tin trạng thái vây hãm lâu đài hiện tại.

## Hành động phía server

Máy chủ trả về trạng thái của sự kiện vây hãm lâu đài.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xB2 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x00 | Tiêu đề gói - mã định danh loại gói phụ |