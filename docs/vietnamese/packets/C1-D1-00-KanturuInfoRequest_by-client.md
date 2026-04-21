# C1 D1 00 - KanturuInfoRequest (client gửi)

## Được gửi khi nào

Người chơi nói chuyện với npc lối vào kanturu và hiển thị hộp thoại nhập.

## Hành động phía server

Máy chủ trả về dữ liệu về trạng thái của bản đồ sự kiện kanturu.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xD1 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x00 | Tiêu đề gói - mã định danh loại gói phụ |