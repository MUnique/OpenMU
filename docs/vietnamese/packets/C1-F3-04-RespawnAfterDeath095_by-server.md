# C1 F3 04 - RespawnAfterDeath095 (server gửi)

## Được gửi khi nào

Nhân vật đã hồi sinh sau khi chết.

## Hành động phía client

Nhân vật hồi sinh với các thuộc tính được chỉ định tại map được chỉ định.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 22 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xF3 | Packet header - packet type identifier |
| 3 | 1 | Byte | 0x04 | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | PositionX |
| 5 | 1 | Byte |  | PositionY |
| 6 | 1 | Byte |  | MapNumber |
| 7 | 1 | Byte |  | Direction |
| 8 | 2 | ShortLittleEndian |  | CurrentHealth |
| 10 | 2 | ShortLittleEndian |  | CurrentMana |
| 12 | 2 | ShortLittleEndian |  | CurrentAbility |
| 14 | 4 | IntegerLittleEndian |  | Experience |
| 18 | 4 | IntegerLittleEndian |  | Money |
