# C3 CB - FriendInvitationResult (server gửi)

## Được gửi khi nào

Người chơi đã yêu cầu thêm người chơi khác vào danh sách bạn bè của mình và máy chủ đã xử lý yêu cầu này.

## Hành động phía client

Ứng dụng khách trò chơi biết liệu lời mời có thể được gửi đến người chơi khác hay không.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 8 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xCB | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Boolean |  | Thành công |
| 4 | 4 | IntegerBigEndian |  | Id yêu cầu |