# C3 AA 07 - DuelChannelJoinRequest (client gửi)

## Được gửi khi nào

Một người chơi yêu cầu tham gia trận đấu với tư cách là khán giả.

## Hành động phía server

Máy chủ sẽ thêm người chơi làm khán giả.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xAA | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x07 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Byte |  | Id kênh |