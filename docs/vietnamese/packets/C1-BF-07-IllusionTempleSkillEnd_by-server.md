# C1 BF 07 - IllusionTempleSkillEnd (server gửi)

## Được gửi khi nào

?

## Hành động phía client

Khách hàng cho thấy các điểm kỹ năng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 6 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xBF | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x07 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Byte |  | Đền ThờSố |
| 5 | 1 | Byte |  | Tình trạng |