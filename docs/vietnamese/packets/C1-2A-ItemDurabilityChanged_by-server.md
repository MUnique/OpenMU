# C1 2A - ItemDurabilityChanged (server gửi)

## Được gửi khi nào

Độ bền của một vật phẩm trong kho của người chơi đã được thay đổi.

## Hành động phía client

Khách hàng cập nhật mặt hàng trong giao diện người dùng kho.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 6 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x2A | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte |  | Khe hàng tồn kho |
| 4 | 1 | Byte |  | Độ bền |
| 5 | 1 | Boolean |  | BởiTiêu thụ; đúng, nếu thay đổi xuất phát từ việc tiêu thụ một mặt hàng; nếu không thì sai |