# C1 AA 0D - DuelHealthBarInit (server gửi)

## Được gửi khi nào

Khi trận đấu bắt đầu, sau thông báo DuelInit.

## Hành động phía client

Khách hàng cập nhật các thanh sức khỏe và lá chắn được hiển thị.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xAA | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x0D | Tiêu đề gói - mã định danh loại gói phụ |