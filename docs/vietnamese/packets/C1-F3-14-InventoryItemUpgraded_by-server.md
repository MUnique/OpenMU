# C1 F3 14 - InventoryItemUpgraded (server gửi)

## Được gửi khi nào

Một vật phẩm trong kho đã được người chơi nâng cấp, ví dụ: bằng cách áp dụng một viên ngọc.

## Hành động phía client

Mục được cập nhật trên giao diện người dùng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte |  | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF3 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x14 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Byte |  | Khe hàng tồn kho |
| 5 |  | Binary |  | Dữ liệu mục |