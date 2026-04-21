# C3 4A - RageAttack (server gửi)

## Được gửi khi nào

Một người chơi (chiến binh cuồng nộ) thực hiện kỹ năng mặt tối lên mục tiêu và gửi RageAttackRangeRequest.

## Hành động phía client

Các mục tiêu bị tấn công bằng hiệu ứng hình ảnh.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 9 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x4A | Tiêu đề gói - mã định danh loại gói |
| 3 | 2 | ShortBigEndian |  | ID kỹ năng |
| 5 | 2 | ShortBigEndian |  | Id nguồn |
| 7 | 2 | ShortBigEndian |  | Id mục tiêu |