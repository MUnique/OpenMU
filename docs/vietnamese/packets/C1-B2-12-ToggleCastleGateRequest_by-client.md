# C1 B2 12 - ToggleCastleGateRequest (client gửi)

## Được gửi khi nào

Thành viên hội của chủ lâu đài muốn bật công tắc cổng.

## Hành động phía server

Cổng lâu đài đang mở hoặc đóng lại.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 7 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xB2 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x12 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Boolean |  | ĐóngTrạng thái |
| 5 | 2 | ShortBigEndian |  | ID cổng |