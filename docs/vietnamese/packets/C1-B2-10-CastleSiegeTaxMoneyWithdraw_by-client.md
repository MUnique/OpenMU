# C1 B2 10 - CastleSiegeTaxMoneyWithdraw (client gửi)

## Được gửi khi nào

Hội trưởng muốn rút tiền thuế từ NPC lâu đài.

## Hành động phía server

Máy chủ chuyển tiền vào kho của chủ bang hội.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 8 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xB2 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x10 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 4 | IntegerBigEndian |  | Số lượng |