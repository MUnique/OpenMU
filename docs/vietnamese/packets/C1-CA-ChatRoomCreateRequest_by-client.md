# C1 CA - ChatRoomCreateRequest (client gửi)

## Được gửi khi nào

Một người chơi muốn mở cuộc trò chuyện với người chơi khác trong danh sách bạn bè của mình.

## Hành động phía server

Nếu cả hai người chơi đều trực tuyến, một phòng trò chuyện sẽ được tạo trên máy chủ trò chuyện. Dữ liệu xác thực được gửi đến cả hai ứng dụng khách trò chơi, sau đó sẽ cố gắng kết nối với máy chủ trò chuyện bằng dữ liệu này.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 13 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xCA | Tiêu đề gói - mã định danh loại gói |
| 3 | 10 | String |  | Tên bạn bè |