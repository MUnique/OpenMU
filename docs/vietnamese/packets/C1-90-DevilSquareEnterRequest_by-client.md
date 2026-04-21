# C1 90 - DevilSquareEnterRequest (client gửi)

## Được gửi khi nào

Khi người chơi yêu cầu vào event Devil Square thông qua NPC Charon.

## Hành động phía server

Server kiểm tra điều kiện tham gia và gửi phản hồi mã `0x90` về client.
Nếu thành công, nhân vật sẽ được chuyển tới map sự kiện.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0x90 | Packet header - packet type identifier |
| 3 | 1 | Byte |  | SquareLevel; level Devil Square trừ đi 1. |
| 4 | 1 | Byte |  | TicketItemInventoryIndex; index vé trong inventory (lưu ý giá trị cao hơn 12 so với mong đợi). |
