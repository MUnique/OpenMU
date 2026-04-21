# C1 D4 - WalkRequest (client gửi)

## Được gửi khi nào

Khi người chơi muốn di chuyển trên map.

## Hành động phía server

Server cập nhật vị trí người chơi trên map, và các người chơi xung quanh có thể
quan sát được chuyển động này.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte |  | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xD4 | Packet header - packet type identifier |
| 3 | 1 | Byte |  | SourceX |
| 4 | 1 | Byte |  | SourceY |
| 5 | 4 bit | Byte |  | StepCount |
| 5 | 4 bit | Byte |  | TargetRotation |
| 6 |  | Binary |  | Directions; dãy hướng đi của đường di chuyển. Tọa độ đích được tính từ tọa độ nguồn cộng lần lượt các hướng này. |
