# C1 17 - ObjectGotKilled (server gửi)

## Được gửi khi nào

Khi một object trong phạm vi quan sát bị hạ gục.

## Hành động phía client

Client hiển thị object ở trạng thái đã chết.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 9 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0x17 | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | KilledId |
| 5 | 2 | ShortBigEndian |  | SkillId |
| 7 | 2 | ShortBigEndian |  | KillerId |
