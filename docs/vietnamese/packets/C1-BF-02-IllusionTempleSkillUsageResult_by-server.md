# C1 BF 02 - IllusionTempleSkillUsageResult (server gửi)

## Được gửi khi nào

Một người chơi đã yêu cầu sử dụng một kỹ năng cụ thể trong sự kiện ngôi đền ảo ảnh.

## Hành động phía client

Khách hàng hiển thị kết quả.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 11 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xBF | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x02 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Byte |  | Kết quả |
| 5 | 2 | ShortBigEndian |  | Số kỹ năng |
| 7 | 2 | ShortLittleEndian |  | Id đối tượng nguồn |
| 9 | 2 | ShortLittleEndian |  | Id đối tượng mục tiêu |