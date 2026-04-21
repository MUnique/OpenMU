# C1 BF 0A - ChainLightningHitInfo (server gửi)

## Được gửi khi nào

Người chơi áp dụng tia sét chuỗi vào mục tiêu và máy chủ sẽ tính toán số lần trúng đích.

## Hành động phía client

Khách hàng cho thấy hiệu ứng sét dây chuyền.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte |  | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xBF | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x0A | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 2 | ShortBigEndian |  | Số kỹ năng |
| 6 | 2 | ShortLittleEndian |  | Id người chơi |
| 8 | 1 | Byte |  | Số mục tiêu |
| 10 | ChainTarget.Length * TargetCount | Array of ChainTarget |  | Mục tiêu |

### Cấu trúc ChainTarget
Chứa mã định danh mục tiêu.

Độ dài: 2 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortLittleEndian |  | Id mục tiêu |