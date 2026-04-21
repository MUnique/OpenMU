# C3 1C - MapChanged075 (server gửi)

## Được gửi khi nào

Map đã được thay đổi phía server.

## Hành động phía client

Game client chuyển sang map và toạ độ được chỉ định.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 8 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0x1C | Packet header - packet type identifier |
| 3 | 1 | Boolean | true | IsMapChange; Nếu false, hiển thị animation dịch chuyển (bọt trắng), và client không gỡ hết các object trong phạm vi. |
| 4 | 1 | Byte |  | MapNumber |
| 5 | 1 | Byte |  | PositionX |
| 6 | 1 | Byte |  | PositionY |
| 7 | 1 | Byte |  | Rotation |
