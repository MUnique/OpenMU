# C1 05 - KeepAlive (client gửi)

## Được gửi khi nào

Gói này được khách hàng gửi trong một khoảng thời gian cố định.

## Hành động phía server

Máy chủ sẽ giữ nguyên kết nối và phòng trò chuyện miễn là khách hàng gửi tin nhắn trong một khoảng thời gian nhất định.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 3 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x05 | Tiêu đề gói - mã định danh loại gói |