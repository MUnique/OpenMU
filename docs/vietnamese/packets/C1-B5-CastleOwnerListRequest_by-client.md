# C1 B5 - CastleOwnerListRequest (client gửi)

## Được gửi khi nào

Chủ bang hội đã mở một npc và cần danh sách các bang hội hiện tại là chủ sở hữu lâu đài.

## Hành động phía server

Máy chủ trả về danh sách các bang hội là chủ sở hữu lâu đài.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 3 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xB5 | Tiêu đề gói - mã định danh loại gói |