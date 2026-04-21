# C1 71 - PingResponse (client gửi)

## Được gửi khi nào

Sau khi máy chủ gửi yêu cầu ping.

## Hành động phía server

Máy chủ biết độ trễ giữa máy chủ và máy khách.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 3 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x71 | Tiêu đề gói - mã định danh loại gói |