# C1 AA 08 - DuelSpectatorAdded (server gửi)

## Được gửi khi nào

Khi một khán giả tham gia một cuộc đấu tay đôi.

## Hành động phía client

Khách hàng cập nhật danh sách người xem.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 14 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xAA | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x08 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 10 | String |  | Name |