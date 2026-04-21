# C1 C3 - FriendDelete (client gửi)

## Được gửi khi nào

Khi người chơi muốn xóa một nhân vật khỏi danh sách bạn bè trong messenger.

## Hành động phía server

Server xóa mục tương ứng trong danh sách bạn bè. Người chơi bị xóa sẽ được
hiển thị offline trong danh sách bạn bè của bên còn lại.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 13 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xC3 | Packet header - packet type identifier |
| 3 | 10 | String |  | FriendName |
