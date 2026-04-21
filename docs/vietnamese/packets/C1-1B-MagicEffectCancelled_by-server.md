# C1 1B - MagicEffectCancelled (server gửi)

## Được gửi khi nào

Người chơi đã hủy hiệu ứng ma thuật cụ thể của một kỹ năng (Mũi tên vô cực, Tăng cường pháp thuật) hoặc một hiệu ứng bị xóa do hết thời gian chờ (Băng, Độc) hoặc thuốc giải độc.

## Hành động phía client

Hiệu ứng được loại bỏ khỏi đối tượng mục tiêu.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 7 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x1B | Tiêu đề gói - mã định danh loại gói |
| 3 | 2 | ShortBigEndian |  | ID kỹ năng |
| 5 | 2 | ShortBigEndian |  | Id mục tiêu |