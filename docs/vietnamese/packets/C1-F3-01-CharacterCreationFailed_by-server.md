# C1 F3 01 - CharacterCreationFailed (server gửi)

## Được gửi khi nào

Sau khi server xử lý yêu cầu tạo nhân vật nhưng không thành công.

## Hành động phía client

Hiển thị thông báo tạo nhân vật thất bại.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xF3 | Packet header - packet type identifier |
| 3 | 1 | Byte | 0x01 | Packet header - sub packet type identifier |
