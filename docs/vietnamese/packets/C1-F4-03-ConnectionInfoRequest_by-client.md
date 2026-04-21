# C1 F4 03 - ConnectionInfoRequest (client gửi)

## Được gửi khi nào

Packet này được client gửi sau khi người dùng bấm vào một server trong danh sách.

## Hành động phía server

Server gửi `ConnectionInfo` trả về cho client.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 6 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xF4 | Packet header - packet type identifier |
| 3 | 1 | Byte | 0x03 | Packet header - sub packet type identifier |
| 4 | 2 | ShortLittleEndian |  | ServerId |
