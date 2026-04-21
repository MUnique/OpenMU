# C1 AA 05 - DuelHealthUpdate (server gửi)

## Được gửi khi nào

Khi sức khỏe/lá chắn của người chơi đấu tay đôi đã được thay đổi.

## Hành động phía client

Khách hàng cập nhật các thanh sức khỏe và lá chắn được hiển thị.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 12 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xAA | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x05 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 2 | ShortBigEndian |  | Id người chơi1 |
| 6 | 2 | ShortBigEndian |  | Id người chơi2 |
| 8 | 1 | Byte |  | Player1Phần trăm sức khỏe |
| 9 | 1 | Byte |  | Player2HealthPhần trăm |
| 10 | 1 | Byte |  | Phần trăm Player1Shield |
| 11 | 1 | Byte |  | Player2ShieldPhần trăm |