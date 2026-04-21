# C1 D0 0A - MoveToDeviasBySnowmanRequest (client gửi)

## Được gửi khi nào

Người chơi nói chuyện với npc "Snowman" ở Làng Santa và yêu cầu quay trở lại devias.

## Hành động phía server

Người chơi sẽ bị dịch chuyển trở lại Devias.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xD0 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x0A | Tiêu đề gói - mã định danh loại gói phụ |