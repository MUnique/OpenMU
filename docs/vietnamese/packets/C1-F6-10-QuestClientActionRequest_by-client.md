# C1 F6 10 - QuestClientActionRequest (client gửi)

## Được gửi khi nào

Ứng dụng khách trò chơi yêu cầu hoàn thành một hành động của khách hàng, ví dụ: hoàn thành một hướng dẫn.

## Hành động phía server

Máy chủ kiểm tra xem nhiệm vụ được chỉ định có đang được thực hiện hay không. Nếu nhiệm vụ có Điều kiện (loại điều kiện 0x10) cho cờ này thì điều kiện được gắn cờ là đã hoàn thành.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 8 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF6 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x10 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 2 | ShortLittleEndian |  | Số nhiệm vụ |
| 6 | 2 | ShortLittleEndian |  | Nhóm nhiệm vụ |