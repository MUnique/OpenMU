# C1 C0 - FriendListRequest (client gửi)

## Được gửi khi nào

Client yêu cầu danh sách bạn bè hiện tại.

## Hành động phía server

Máy chủ gửi danh sách bạn bè cho máy khách.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 3 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xC0 | Tiêu đề gói - mã định danh loại gói |