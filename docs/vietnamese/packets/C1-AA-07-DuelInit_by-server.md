# C1 AA 07 - DuelInit (server gửi)

## Được gửi khi nào

Khi cuộc đấu tay đôi bắt đầu.

## Hành động phía client

Máy khách khởi tạo trạng thái đấu tay đôi.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 30 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xAA | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x07 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Byte |  | Kết quả |
| 5 | 1 | Byte |  | Phòng mục lục |
| 6 | 10 | String |  | Tên người chơi1 |
| 16 | 10 | String |  | Tên người chơi2 |
| 26 | 2 | ShortBigEndian |  | Id người chơi1 |
| 28 | 2 | ShortBigEndian |  | Id người chơi2 |