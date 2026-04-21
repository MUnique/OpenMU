# C1 38 - TradeItemRemoved (server gửi)

## Được gửi khi nào

Đối tác thương mại đã loại bỏ một mặt hàng khỏi giao dịch.

## Hành động phía client

Mục này sẽ bị xóa khỏi hộp thoại giao dịch.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x38 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte |  | Chỗ |