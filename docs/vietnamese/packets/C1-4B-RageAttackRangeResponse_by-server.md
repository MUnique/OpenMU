# C1 4B - RageAttackRangeResponse (server gửi)

## Được gửi khi nào

Một người chơi (chiến binh cuồng nộ) thực hiện kỹ năng mặt tối lên mục tiêu và gửi RageAttackRangeRequest.

## Hành động phía client

Các mục tiêu bị tấn công bằng hiệu ứng hình ảnh.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 16 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x4B | Tiêu đề gói - mã định danh loại gói |
| 4 | 2 | ShortLittleEndian |  | ID kỹ năng |
| 6 | RageTarget.Length * | Array of RageTarget |  | Mục tiêu |

### Cấu trúc RageTarget
Chứa mã định danh mục tiêu.

Độ dài: 2 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortLittleEndian | 10000 | Id mục tiêu |