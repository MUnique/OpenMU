# C1 F3 12 - ClientReadyAfterMapChange (client gửi)

## Được gửi khi nào

Sau khi máy chủ gửi thông báo thay đổi bản đồ và máy khách đã khởi tạo hiển thị bản đồ trò chơi.

## Hành động phía server

Nhân vật được thêm vào bản đồ trò chơi nội bộ và sẵn sàng tương tác với các thực thể khác.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF3 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x12 | Tiêu đề gói - mã định danh loại gói phụ |