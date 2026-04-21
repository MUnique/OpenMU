# C3 CA - ChatRoomConnectionInfo (server gửi)

## Được gửi khi nào

Người chơi được mời tham gia phòng trò chuyện trên máy chủ trò chuyện.

## Hành động phía client

Ứng dụng khách trò chơi kết nối với máy chủ trò chuyện và tham gia phòng trò chuyện với dữ liệu xác thực được chỉ định.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 36 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xCA | Tiêu đề gói - mã định danh loại gói |
| 3 | 15 | String |  | Ip máy chủ trò chuyện |
| 18 | 2 | ShortLittleEndian |  | Id phòng trò chuyện |
| 20 | 4 | IntegerLittleEndian |  | Mã thông báo xác thực |
| 24 | 1 | Byte | 1 | Type |
| 25 | 10 | String |  | Tên bạn bè |
| 35 | 1 | Boolean |  | Thành công |