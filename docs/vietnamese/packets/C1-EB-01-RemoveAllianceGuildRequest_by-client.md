# C1 EB 01 - RemoveAllianceGuildRequest (client gửi)

## Được gửi khi nào

Chủ bang hội liên minh muốn loại bỏ bang hội khỏi liên minh.

## Hành động phía server

Máy chủ loại bỏ bang hội khỏi liên minh.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 12 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xEB | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x01 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 |  | String |  | Tên bang hội |