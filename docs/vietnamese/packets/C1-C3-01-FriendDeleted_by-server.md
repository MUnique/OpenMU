# C1 C3 01 - FriendDeleted (server gửi)

## Được gửi khi nào

Sau khi một bạn bè bị xóa khỏi danh sách bạn bè.

## Hành động phía client

Xóa bạn bè đó khỏi danh sách bạn bè trên client.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 14 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xC3 | Packet header - packet type identifier |
| 3 | 1 | Byte | 0x01 | Packet header - sub packet type identifier |
| 4 | 10 | String |  | FriendName |
