# C1 C2 - FriendAddResponse (client gửi)

## Được gửi khi nào

Một người chơi đã nhận được yêu cầu kết bạn từ một người chơi khác và đã phản hồi lại yêu cầu đó.

## Hành động phía server

Nếu người chơi chấp nhận, người bạn đó sẽ được thêm vào danh sách bạn bè của người chơi và cả hai người chơi sẽ đăng ký trạng thái trực tuyến của nhau.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 14 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xC2 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Boolean |  | Đã chấp nhận |
| 4 | 10 | String |  | Tên người yêu cầu bạn bè |