# C1 04 - ChatMessage

## Được gửi khi nào

Gói này được máy chủ gửi sau khi một ứng dụng trò chuyện khác gửi tin nhắn đến phòng trò chuyện hiện tại.

## Hành động phía client

Khách hàng sẽ hiển thị thông báo.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte |  | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x04 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte |  | Chỉ mục người gửi |
| 4 | 1 | Byte |  | Độ dài tin nhắn |
| 5 |  | Binary |  | Tin nhắn; Tin nhắn. Nó được "mã hóa" bằng khóa XOR 3 byte (FC CF AB). |