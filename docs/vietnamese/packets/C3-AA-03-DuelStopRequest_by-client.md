# C3 AA 03 - DuelStopRequest (client gửi)

## Được gửi khi nào

Một người chơi yêu cầu dừng cuộc đấu tay đôi.

## Hành động phía server

Máy chủ dừng cuộc đấu tay đôi.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xAA | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x03 | Tiêu đề gói - mã định danh loại gói phụ |