# C3 19 - SkillAnimation (server gửi)

## Được gửi khi nào

Một đối tượng thực hiện một kỹ năng nhắm trực tiếp vào một đối tượng khác.

## Hành động phía client

Hình ảnh động được hiển thị trên giao diện người dùng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 9 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x19 | Tiêu đề gói - mã định danh loại gói |
| 3 | 2 | ShortBigEndian |  | ID kỹ năng |
| 5 | 2 | ShortBigEndian |  | Id người chơi |
| 7 | 2 | ShortBigEndian |  | Id mục tiêu |