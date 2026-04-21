# C1 CB - ChatRoomInvitationRequest (client gửi)

## Được gửi khi nào

Một người chơi muốn mời thêm người chơi từ danh sách bạn bè của mình vào phòng trò chuyện hiện có.

## Hành động phía server

Người chơi cũng nhận được dữ liệu xác thực được gửi đến ứng dụng khách trò chơi của mình. Sau đó nó kết nối với máy chủ trò chuyện và tham gia phòng trò chuyện.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 19 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xCB | Tiêu đề gói - mã định danh loại gói |
| 3 | 10 | String |  | Tên bạn bè |
| 13 | 2 | ShortBigEndian |  | Mã phòng |
| 15 | 4 | IntegerBigEndian |  | Id yêu cầu |