# C1 F4 03 - ConnectionInfoRequest075 (client gửi)

## Được gửi khi nào

Packet này được client gửi sau khi người chơi click vào một mục trong danh sách server.

## Hành động phía server

Server sẽ gửi lại ConnectionInfo cho client.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xF4 | Packet header - packet type identifier |
| 3 | 1 | Byte | 0x03 | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | ServerId |
