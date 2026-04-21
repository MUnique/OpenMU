# C1 10 - ObjectWalked075 (server gửi)

## Được gửi khi nào

Một đối tượng trong phạm vi quan sát (bao gồm cả chính người chơi) đã di chuyển sang vị trí khác.

## Hành động phía client

Đối tượng được animate để di chuyển đến vị trí mới.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 8 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0x10 | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | ObjectId |
| 5 | 1 | Byte |  | TargetX |
| 6 | 1 | Byte |  | TargetY |
| 7 | 4 bit | Byte |  | TargetRotation |
