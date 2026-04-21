# C1 F3 22 - GuildSoccerTimeUpdate (server gửi)

## Được gửi khi nào

Mỗi giây trong một trận đấu bóng đá của bang hội.

## Hành động phía client

Thời gian được cập nhật trên giao diện người dùng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 6 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF3 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x22 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 2 | ShortLittleEndian |  | Giây |