# C1 AA 04 - DuelScore (server gửi)

## Được gửi khi nào

Khi tỉ số của trận đấu đã được thay đổi.

## Hành động phía client

Khách hàng cập nhật điểm đấu tay đôi được hiển thị.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 10 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xAA | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x04 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 2 | ShortBigEndian |  | Id người chơi1 |
| 6 | 2 | ShortBigEndian |  | Id người chơi2 |
| 8 | 1 | Byte |  | Người chơi1Điểm |
| 9 | 1 | Byte |  | Người chơi2Score |