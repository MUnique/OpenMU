# C1 F3 03 - SelectCharacter (client gửi)

## Được gửi khi nào

Người chơi chọn một nhân vật ở màn hình chọn nhân vật để vào game world.

## Hành động phía server

Người chơi vào game world bằng nhân vật đã chọn.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 14 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xF3 | Packet header - packet type identifier |
| 3 | 1 | Byte | 0x03 | Packet header - sub packet type identifier |
| 4 | 10 | String |  | Name; tên nhân vật mà người chơi muốn dùng để vào game world |
