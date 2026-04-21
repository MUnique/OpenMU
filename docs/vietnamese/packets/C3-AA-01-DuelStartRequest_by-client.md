# C3 AA 01 - DuelStartRequest (client gửi)

## Được gửi khi nào

Người chơi yêu cầu bắt đầu đấu tay đôi với người chơi khác.

## Hành động phía server

Máy chủ gửi yêu cầu đến người chơi khác.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 16 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xAA | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x01 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 2 | ShortBigEndian |  | Id người chơi |
| 6 | 10 | String |  | Tên người chơi |