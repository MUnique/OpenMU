# C1 10 - WalkRequest075 (client gửi)

## Được gửi khi nào

Khi người chơi muốn di chuyển trên bản đồ.

## Hành động phía server

Người chơi được di chuyển trên map, các người chơi xung quanh sẽ thấy.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte |  | Packet header - độ dài packet |
| 2 | 1 | Byte | 0x10 | Packet header - packet type identifier |
| 3 | 1 | Byte |  | SourceX |
| 4 | 1 | Byte |  | SourceY |
| 5 | 4 bit | Byte |  | StepCount |
| 5 | 4 bit | Byte |  | TargetRotation |
| 6 |  | Binary |  | Directions; Hướng của đường đi. Toạ đích được tính bằng cách lấy toạ độ nguồn và áp dụng các hướng này. |
