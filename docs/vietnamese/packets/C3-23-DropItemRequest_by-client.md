# C3 23 - DropItemRequest (client gửi)

## Được gửi khi nào

Khi người chơi yêu cầu thả một item từ inventory xuống mặt đất.

## Hành động phía server

Nếu tọa độ hợp lệ và item được phép thả, server sẽ thả item xuống map và thông
báo cho các người chơi xung quanh.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 6 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0x23 | Packet header - packet type identifier |
| 3 | 1 | Byte |  | TargetX |
| 4 | 1 | Byte |  | TargetY |
| 5 | 1 | Byte |  | ItemSlot |
