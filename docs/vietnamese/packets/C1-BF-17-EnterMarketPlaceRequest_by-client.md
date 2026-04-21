# C1 BF 17 - EnterMarketPlaceRequest (client gửi)

## Được gửi khi nào

Người chơi muốn vào bản đồ chợ.

## Hành động phía server

Máy chủ di chuyển người chơi đến bản đồ khu chợ.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xBF | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x17 | Tiêu đề gói - mã định danh loại gói phụ |