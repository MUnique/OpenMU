# C1 B2 06 - CastleSiegeDefenseRepairRequest (client gửi)

## Được gửi khi nào

Người chơi mở một npc bao vây lâu đài và yêu cầu sửa chữa một cánh cổng hoặc bức tượng ở một vị trí cụ thể (chỉ mục)..

## Hành động phía server

Máy chủ trả về phản hồi.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 12 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xB2 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x06 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 4 | IntegerLittleEndian |  | Số Npc |
| 8 | 4 | IntegerLittleEndian |  | Chỉ số Npc |