# C1 64 - GuildWarScoreUpdate (server gửi)

## Được gửi khi nào

Điểm chiến tranh bang hội đã thay đổi.

## Hành động phía client

Điểm bang hội được cập nhật ở phía khách hàng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 6 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x64 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte |  | Điểm Của Bang Hội Riêng |
| 4 | 1 | Byte |  | Điểm Của Kẻ ThùGuild |
| 5 | 1 | Byte | 0 | Type |