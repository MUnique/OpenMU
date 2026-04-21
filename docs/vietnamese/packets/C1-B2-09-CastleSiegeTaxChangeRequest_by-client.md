# C1 B2 09 - CastleSiegeTaxChangeRequest (client gửi)

## Được gửi khi nào

Hội trưởng muốn thay đổi thuế suất trong lâu đài NPC.

## Hành động phía server

Máy chủ thay đổi mức thuế tương ứng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 9 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xB2 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x09 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Byte |  | Loại thuế; 0=Không xác định, 1=ChaosMachine, 2 = Bình thường, 3 = Phí vào cửaLandOfTrials |
| 5 | 4 | IntegerBigEndian |  | Thuế suất |