# C1 01 01 - ChatRoomClientLeft (server gửi)

## Được gửi khi nào

Gói này được máy chủ gửi sau khi ứng dụng khách trò chuyện rời khỏi phòng trò chuyện.

## Hành động phía client

Máy khách sẽ xóa máy khách khỏi danh sách hoặc đánh dấu tên của nó trên thanh tiêu đề là ngoại tuyến.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 15 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x01 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x01 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Byte |  | Chỉ mục khách hàng |
| 5 | 10 | String |  | Name |