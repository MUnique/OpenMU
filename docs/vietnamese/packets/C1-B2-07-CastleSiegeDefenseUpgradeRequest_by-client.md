# C1 B2 07 - CastleSiegeDefenseUpgradeRequest (client gửi)

## Được gửi khi nào

Người chơi mở npc bao vây lâu đài và yêu cầu nâng cấp cổng hoặc tượng tại một vị trí (chỉ số) cụ thể..

## Hành động phía server

Máy chủ trả về phản hồi.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 20 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xB2 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x07 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 4 | IntegerLittleEndian |  | Số Npc |
| 8 | 4 | IntegerLittleEndian |  | Chỉ số Npc |
| 12 | 4 | IntegerLittleEndian |  | Loại nâng cấp Npc |
| 16 | 4 | IntegerLittleEndian |  | Giá trị nâng cấp Npc |