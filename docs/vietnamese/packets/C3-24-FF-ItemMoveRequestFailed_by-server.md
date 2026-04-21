# C3 24 FF - ItemMoveRequestFailed (server gửi)

## Được gửi khi nào

Không thể di chuyển một vật phẩm trong kho hoặc kho tiền của người chơi theo yêu cầu của người chơi.

## Hành động phía client

Máy khách khôi phục vị trí của mục trong giao diện người dùng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte |  | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x24 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0xFF | Tiêu đề gói - mã định danh loại gói phụ |
| 5 |  | Binary |  | Dữ liệu mục |