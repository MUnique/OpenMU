# C1 1B - MagicEffectCancelRequest (client gửi)

## Được gửi khi nào

Người chơi hủy bỏ hiệu ứng ma thuật cụ thể của một kỹ năng, thường là 'Mũi tên vô cực' và 'Tăng cường pháp thuật'.

## Hành động phía server

Hiệu ứng bị hủy và bản cập nhật được gửi đến người chơi và tất cả người chơi xung quanh.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 7 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x1B | Tiêu đề gói - mã định danh loại gói |
| 3 | 2 | ShortBigEndian |  | ID kỹ năng |
| 5 | 2 | ShortBigEndian |  | Id người chơi |