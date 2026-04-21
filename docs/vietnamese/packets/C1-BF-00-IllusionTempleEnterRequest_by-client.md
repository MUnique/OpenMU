# C1 BF 00 - IllusionTempleEnterRequest (client gửi)

## Được gửi khi nào

Khi client đang mở hội thoại NPC Illusion Temple và muốn vào map sự kiện.

## Hành động phía server

Server kiểm tra vé yêu cầu và chuyển người chơi vào map sự kiện nếu hợp lệ.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 6 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xBF | Packet header - packet type identifier |
| 3 | 1 | Byte | 0x00 | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | MapNumber |
| 5 | 1 | Byte |  | ItemSlot |
