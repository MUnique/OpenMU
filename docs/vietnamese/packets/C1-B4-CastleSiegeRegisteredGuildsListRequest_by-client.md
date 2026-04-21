# C1 B4 - CastleSiegeRegisteredGuildsListRequest (client gửi)

## Được gửi khi nào

Chủ bang hội đã mở một npc và cần danh sách các bang hội đã đăng ký cho cuộc vây hãm tiếp theo.

## Hành động phía server

Máy chủ trả về danh sách bang hội cho cuộc vây hãm tiếp theo.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 3 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xB4 | Tiêu đề gói - mã định danh loại gói |