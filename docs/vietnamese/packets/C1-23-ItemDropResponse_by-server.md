# C1 23 - ItemDropResponse (server gửi)

## Được gửi khi nào

Khi người chơi gửi yêu cầu thả item từ inventory; packet này trả về kết quả
thành công/thất bại.

## Hành động phía client

Nếu thành công, client xóa item khỏi giao diện inventory.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0x23 | Packet header - packet type identifier |
| 3 | 1 | Boolean |  | Success |
| 4 | 1 | Byte |  | InventorySlot |
