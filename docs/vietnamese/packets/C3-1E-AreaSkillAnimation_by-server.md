# C3 1E - AreaSkillAnimation (server gửi)

## Được gửi khi nào

Một đối tượng thực hiện một kỹ năng có tác dụng lên một khu vực.

## Hành động phía client

Hình ảnh động được hiển thị trên giao diện người dùng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 10 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x1E | Tiêu đề gói - mã định danh loại gói |
| 3 | 2 | ShortBigEndian |  | ID kỹ năng |
| 5 | 2 | ShortBigEndian |  | Id người chơi |
| 7 | 1 | Byte |  | điểmX |
| 8 | 1 | Byte |  | điểm Y |
| 9 | 1 | Byte |  | Xoay |