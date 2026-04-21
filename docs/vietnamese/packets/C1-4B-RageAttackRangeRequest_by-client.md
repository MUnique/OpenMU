# C1 4B - RageAttackRangeRequest (client gửi)

## Được gửi khi nào

Một người chơi (chiến binh cuồng nộ) thực hiện kỹ năng mặt tối lên mục tiêu.

## Hành động phía server

Các mục tiêu (tối đa 5) được xác định và gửi lại cho người chơi bằng RageAttackRangeResponse.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 7 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x4B | Tiêu đề gói - mã định danh loại gói |
| 3 | 2 | ShortBigEndian |  | ID kỹ năng |
| 5 | 2 | ShortBigEndian |  | Id mục tiêu |