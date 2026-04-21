# C1 07 - MagicEffectStatus (server gửi)

## Được gửi khi nào

Một hiệu ứng ma thuật đã được thêm vào hoặc loại bỏ đối với chính người chơi hoặc người chơi khác.

## Hành động phía client

Giao diện người dùng tự cập nhật. Nếu đó là hiệu ứng của trình phát riêng thì nó sẽ hiển thị dưới dạng biểu tượng ở đầu giao diện.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 7 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x07 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Boolean |  | Đang hoạt động |
| 4 | 2 | ShortBigEndian |  | Id người chơi |
| 6 | 1 | Byte |  | Id hiệu ứng |