# C1 C4 - SetFriendOnlineState (client gửi)

## Được gửi khi nào

Người chơi muốn tự mình bật hoặc ngoại tuyến.

## Hành động phía server

Tùy thuộc vào trạng thái, người chơi được hiển thị ở trạng thái ngoại tuyến hoặc trực tuyến trong tất cả danh sách bạn bè của bạn bè mình.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xC4 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Boolean |  | Trạng thái trực tuyến |