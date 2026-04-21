# C1 01 - LeaveChatRoom (client gửi)

## Được gửi khi nào

Gói này được khách hàng gửi khi rời khỏi phòng trò chuyện, trước khi kết nối đóng lại.

## Hành động phía server

Máy chủ sẽ xóa client khỏi phòng chat, thông báo cho các client còn lại.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 3 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x01 | Tiêu đề gói - mã định danh loại gói |