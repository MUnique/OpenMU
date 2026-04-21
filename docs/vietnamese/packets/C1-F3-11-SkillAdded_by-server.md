# C1 F3 11 - SkillAdded (server gửi)

## Được gửi khi nào

Sau khi một kỹ năng được thêm vào danh sách kỹ năng, ví dụ: bằng cách trang bị một vật phẩm hoặc học một kỹ năng.

## Hành động phía client

Kỹ năng này được thêm vào danh sách kỹ năng ở phía khách hàng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 10 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF3 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x11 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Byte | 0xFE | Lá cờ |
| 6 | 1 | Byte |  | Chỉ số kỹ năng |
| 7 | 2 | ShortLittleEndian |  | Số kỹ năng |
| 9 | 1 | Byte |  | Cấp độ kỹ năng |