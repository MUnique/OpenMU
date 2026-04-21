# C1 01 00 - ChatRoomClientJoined (server gửi)

## Được gửi khi nào

Gói này được máy chủ gửi sau khi một ứng dụng trò chuyện khác tham gia phòng trò chuyện.

## Hành động phía client

Máy khách sẽ thêm máy khách vào danh sách của mình (nếu có trên 2 máy khách được kết nối vào cùng một phòng) hoặc hiển thị tên của máy khách đó trên thanh tiêu đề.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 15 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x01 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x00 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Byte |  | Chỉ mục khách hàng |
| 5 | 10 | String |  | Name |