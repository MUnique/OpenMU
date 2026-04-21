# C1 F1 00 - GameServerEntered (server gửi)

## Được gửi khi nào

Sau khi game client đã kết nối vào game.

## Hành động phía client

Client hiển thị hộp thoại đăng nhập.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 12 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xF1 | Packet header - packet type identifier |
| 3 | 1 | Byte | 0x00 | Packet header - sub packet type identifier |
| 4 | 1 | Boolean | true | Success |
| 5 | 2 | ShortBigEndian |  | PlayerId |
| 7 | 5 | String |  | VersionString |
| 7 | 5 | Binary |  | Version |
