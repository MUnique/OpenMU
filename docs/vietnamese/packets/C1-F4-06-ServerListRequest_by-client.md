# C1 F4 06 - ServerListRequest (client gửi)

## Được gửi khi nào

Packet này được client gửi sau khi kết nối và nhận được message `Hello`.

## Hành động phía server

Server gửi `ServerListResponse` trả về cho client.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xF4 | Packet header - packet type identifier |
| 3 | 1 | Byte | 0x06 | Packet header - sub packet type identifier |
