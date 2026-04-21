# C1 D4 - ObjectWalked (server gửi)

## Được gửi khi nào

Khi một object trong phạm vi quan sát (bao gồm chính người chơi) di chuyển sang
vị trí mới.

## Hành động phía client

Client phát animation di chuyển của object đến vị trí mới.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte |  | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xD4 | Packet header - packet type identifier |
| 2 | 1 | Byte |  | HeaderCode |
| 3 | 2 | ShortBigEndian |  | ObjectId |
| 5 | 1 | Byte |  | TargetX |
| 6 | 1 | Byte |  | TargetY |
| 7 | 4 bit | Byte |  | TargetRotation |
| 7 | 4 bit | Byte |  | StepCount |
| 8 |  | Binary |  | StepData |
