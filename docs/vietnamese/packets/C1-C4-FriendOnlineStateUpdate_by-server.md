# C1 C4 - FriendOnlineStateUpdate (server gửi)

## Được gửi khi nào

Sau khi một người bạn đã được thêm vào danh sách bạn bè.

## Hành động phía client

Người bạn đó xuất hiện trong danh sách bạn bè.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 14 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xC4 | Tiêu đề gói - mã định danh loại gói |
| 3 | 10 | String |  | Tên bạn bè |
| 13 | 1 | Byte |  | Id máy chủ; Id máy chủ mà người chơi hiện đang trực tuyến. 0xFF có nghĩa là ngoại tuyến. |