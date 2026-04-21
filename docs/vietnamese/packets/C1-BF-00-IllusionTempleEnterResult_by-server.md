# C1 BF 00 - IllusionTempleEnterResult (server gửi)

## Được gửi khi nào

Khi người chơi gửi yêu cầu vào sự kiện Illusion Temple.

## Hành động phía client

Client hiển thị kết quả yêu cầu.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xBF | Packet header - packet type identifier |
| 3 | 1 | Byte | 0x00 | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Result |
