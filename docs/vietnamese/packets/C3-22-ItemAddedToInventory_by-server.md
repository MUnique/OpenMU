# C3 22 - ItemAddedToInventory (server gửi)

## Được gửi khi nào

Khi có item mới được thêm vào inventory.

## Hành động phía client

Client thêm item vào giao diện inventory.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte |  | Packet header - độ dài packet |
| 2 | 1 | Byte | 0x22 | Packet header - packet type identifier |
| 3 | 1 | Byte |  | InventorySlot |
| 4 |  | Binary |  | ItemData |
