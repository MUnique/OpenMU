# C1 26 FD - ItemConsumptionFailed (server gửi)

## Được gửi khi nào

Khi việc tiêu thụ một mặt hàng không thành công.

## Hành động phía client

Ứng dụng khách trò chơi nhận được phản hồi về việc tiêu thụ không thành công và cho phép thực hiện các yêu cầu tiêu thụ tiếp theo.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 9 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x26 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0xFD | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 2 | ShortBigEndian |  | Sức khỏe |
| 7 | 2 | ShortBigEndian |  | Khiên |