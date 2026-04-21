# C1 E2 - GuildTypeChangeRequest (client gửi)

## Được gửi khi nào

Chủ bang hội muốn thay đổi loại bang hội của mình. Không tìm thấy bất kỳ vị trí nào trong ứng dụng khách nơi nó được gửi.

## Hành động phía server

Máy chủ thay đổi loại bang hội. Chúng tôi cho rằng liệu bang hội có nên là bang hội chính của liên minh hay không. Không nên xử lý vì điều này là cố định trong suốt thời gian tồn tại của liên minh.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xE2 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte |  | Loại bang hội; 0 = Chung, 1 = Bảo ​​vệ, FF = Không. |