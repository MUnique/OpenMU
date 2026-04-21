# C1 B2 03 - CastleSiegeRegistrationStateRequest (client gửi)

## Được gửi khi nào

Người chơi đã mở một NPC bao vây lâu đài và yêu cầu nhà nước về việc đăng ký của chính mình.

## Hành động phía server

Máy chủ trả về trạng thái đăng ký cuộc vây hãm lâu đài, bao gồm số lượng nhãn hiệu bang hội đã gửi.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xB2 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x03 | Tiêu đề gói - mã định danh loại gói phụ |