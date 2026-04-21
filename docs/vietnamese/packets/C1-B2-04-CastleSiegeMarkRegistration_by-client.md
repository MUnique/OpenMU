# C1 B2 04 - CastleSiegeMarkRegistration (client gửi)

## Được gửi khi nào

Người chơi đã mở một NPC bao vây lâu đài và thêm nhãn hiệu bang hội vào đăng ký bang hội của mình.

## Hành động phía server

Máy chủ trả về phản hồi, bao gồm số lượng nhãn hiệu bang hội đã gửi.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xB2 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x04 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Byte |  | Mục mục |