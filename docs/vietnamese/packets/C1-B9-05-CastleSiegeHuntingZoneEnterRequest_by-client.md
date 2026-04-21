# C1 B9 05 - CastleSiegeHuntingZoneEnterRequest (client gửi)

## Được gửi khi nào

Một thành viên bang hội của chủ sở hữu lâu đài muốn vào khu vực săn bắn (ví dụ: Vùng đất thử thách).

## Hành động phía server

Máy chủ lấy tiền vào cửa, đưa vào ví thuế và đưa người chơi đến khu vực săn bắn.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 8 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xB9 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x05 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 4 | IntegerLittleEndian |  | Tiền bạc |