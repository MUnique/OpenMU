# C1 B3 01 - CastleSiegeGateListRequest (client gửi)

## Được gửi khi nào

Chủ bang hội đã mở npc lâu đài và khách hàng cần có danh sách tất cả các cổng.

## Hành động phía server

Máy chủ trả về danh sách các cổng và trạng thái của chúng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xB3 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x01 | Tiêu đề gói - mã định danh loại gói phụ |