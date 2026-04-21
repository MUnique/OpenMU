# C1 D1 10 - RaklionStateInfoRequest (client gửi)

## Được gửi khi nào

?

## Hành động phía server

?

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xD1 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x10 | Tiêu đề gói - mã định danh loại gói phụ |