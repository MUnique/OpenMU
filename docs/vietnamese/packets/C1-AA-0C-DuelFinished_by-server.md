# C1 AA 0C - DuelFinished (server gửi)

## Được gửi khi nào

Khi cuộc đấu tay đôi kết thúc.

## Hành động phía client

Khách hàng hiển thị tên người thắng và người thua.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 24 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xAA | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x0C | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 10 | String |  | Người chiến thắng |
| 14 | 10 | String |  | Kẻ thất bại |