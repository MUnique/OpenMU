# C1 B9 02 - GuildLogoOfCastleOwnerRequest (client gửi)

## Được gửi khi nào

Khách hàng yêu cầu logo bang hội của bang hội chủ sở hữu lâu đài hiện tại.

## Hành động phía server

Máy chủ trả về logo bang hội.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xB9 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x02 | Tiêu đề gói - mã định danh loại gói phụ |