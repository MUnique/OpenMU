# C1 D1 01 - KanturuEnterRequest (client gửi)

## Được gửi khi nào

Người chơi yêu cầu vào bản đồ sự kiện kanturu.

## Hành động phía server

Máy chủ sẽ kiểm tra xem có thể truy cập hay không và hành động tương ứng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xD1 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x01 | Tiêu đề gói - mã định danh loại gói phụ |