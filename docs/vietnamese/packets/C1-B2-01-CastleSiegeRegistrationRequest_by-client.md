# C1 B2 01 - CastleSiegeRegistrationRequest (client gửi)

## Được gửi khi nào

Người chơi đã mở một NPC bao vây lâu đài để đăng ký liên minh bang hội của mình.

## Hành động phía server

Máy chủ trả về kết quả đăng ký bao vây lâu đài.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xB2 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x01 | Tiêu đề gói - mã định danh loại gói phụ |