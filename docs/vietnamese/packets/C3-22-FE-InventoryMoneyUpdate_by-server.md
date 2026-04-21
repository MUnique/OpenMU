# C3 22 FE - InventoryMoneyUpdate (server gửi)

## Được gửi khi nào

Số tiền của người chơi trong kho đã được thay đổi và cần cập nhật.

## Hành động phía client

Tiền được cập nhật trong giao diện người dùng hàng tồn kho.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 8 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x22 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0xFE | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 4 | IntegerBigEndian |  | Tiền bạc |