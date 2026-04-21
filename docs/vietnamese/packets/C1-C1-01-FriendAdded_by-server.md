# C1 C1 01 - FriendAdded (server gửi)

## Được gửi khi nào

Sau khi thêm bạn bè thành công vào danh sách bạn bè.

## Hành động phía client

Bạn bè mới xuất hiện trong danh sách bạn bè.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 15 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xC1 | Packet header - packet type identifier |
| 3 | 1 | Byte | 0x01 | Packet header - sub packet type identifier |
| 4 | 10 | String |  | FriendName |
| 14 | 1 | Byte | 0xFF | ServerId; server id nơi người chơi đang online. 0xFF nghĩa là offline. |
