# C1 F4 03 - ConnectionInfo (server gửi)

## Được gửi khi nào

Packet này được server gửi sau khi client yêu cầu thông tin kết nối của một
server (thường sau khi người dùng chọn server).

## Hành động phía client

Client sẽ thử kết nối tới server bằng thông tin được trả về.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 22 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xF4 | Packet header - packet type identifier |
| 3 | 1 | Byte | 0x03 | Packet header - sub packet type identifier |
| 4 | 16 | String |  | IpAddress |
| 20 | 2 | ShortLittleEndian |  | Port |
