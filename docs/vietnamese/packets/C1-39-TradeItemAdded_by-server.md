# C1 39 - TradeItemAdded (server gửi)

## Được gửi khi nào

Đối tác thương mại đã thêm một mặt hàng vào giao dịch.

## Hành động phía client

Mục này được thêm vào hộp thoại giao dịch.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte |  | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x39 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte |  | tới khe cắm |
| 4 |  | Binary |  | Dữ liệu mục |