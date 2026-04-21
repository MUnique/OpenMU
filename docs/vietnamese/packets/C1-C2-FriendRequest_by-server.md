# C1 C2 - FriendRequest (server gửi)

## Được gửi khi nào

Sau khi một người chơi yêu cầu thêm người chơi khác làm bạn bè. Người chơi khác này nhận được thông báo này.

## Hành động phía client

Yêu cầu kết bạn xuất hiện trên giao diện người dùng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 13 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xC2 | Tiêu đề gói - mã định danh loại gói |
| 3 | 10 | String |  | Người yêu cầu |