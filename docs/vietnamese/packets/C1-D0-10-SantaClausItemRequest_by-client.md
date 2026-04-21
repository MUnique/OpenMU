# C1 D0 10 - SantaClausItemRequest (client gửi)

## Được gửi khi nào

Người chơi nói chuyện với npc "Santa Claus" và yêu cầu một vật phẩm.

## Hành động phía server

Vật phẩm sẽ rơi xuống đất.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xD0 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x10 | Tiêu đề gói - mã định danh loại gói phụ |