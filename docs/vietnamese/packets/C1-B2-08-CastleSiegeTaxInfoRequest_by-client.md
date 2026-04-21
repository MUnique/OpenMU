# C1 B2 08 - CastleSiegeTaxInfoRequest (client gửi)

## Được gửi khi nào

Hội trưởng đã mở một NPC bao vây lâu đài để quản lý lâu đài.

## Hành động phía server

Máy chủ trả về thông tin thuế.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xB2 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x08 | Tiêu đề gói - mã định danh loại gói phụ |