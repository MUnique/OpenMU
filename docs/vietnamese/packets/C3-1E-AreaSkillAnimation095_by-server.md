# C3 1E - AreaSkillAnimation095 (server gửi)

## Được gửi khi nào

Một đối tượng thực hiện skill có hiệu ứng trên một vùng.

## Hành động phía client

Animation được hiển thị trên giao diện.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 9 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0x1E | Packet header - packet type identifier |
| 3 | 1 | Byte |  | SkillId |
| 4 | 2 | ShortBigEndian |  | PlayerId |
| 6 | 1 | Byte |  | PointX |
| 7 | 1 | Byte |  | PointY |
| 8 | 1 | Byte |  | Rotation |
