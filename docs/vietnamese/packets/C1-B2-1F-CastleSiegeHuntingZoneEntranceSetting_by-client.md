# C1 B2 1F - CastleSiegeHuntingZoneEntranceSetting (client gửi)

## Được gửi khi nào

Một thành viên bang hội của chủ sở hữu lâu đài muốn vào khu vực săn bắn (ví dụ: Vùng đất thử thách).

## Hành động phía server

Máy chủ thay đổi cài đặt lối vào khu vực săn bắn.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xB2 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x1F | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Boolean |  | Là công khai |