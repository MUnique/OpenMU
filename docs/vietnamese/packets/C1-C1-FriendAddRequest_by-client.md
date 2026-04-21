# C1 C1 - FriendAddRequest (client gửi)

## Được gửi khi nào

Khi người chơi muốn thêm nhân vật của người chơi khác vào danh sách bạn bè
trong messenger.

## Hành động phía server

Server gửi yêu cầu tới người chơi còn lại. Nếu người đó đang offline, yêu cầu
sẽ được gửi khi họ online trở lại.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 13 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xC1 | Packet header - packet type identifier |
| 3 | 10 | String |  | FriendName |
