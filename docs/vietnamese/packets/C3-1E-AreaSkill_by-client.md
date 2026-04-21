# C3 1E - AreaSkill (client gửi)

## Được gửi khi nào

Người chơi đang thực hiện một kỹ năng ảnh hưởng đến một khu vực trên bản đồ.

## Hành động phía server

Nó được chuyển tiếp đến tất cả người chơi xung quanh để có thể nhìn thấy hoạt ảnh. Trong quá trình triển khai máy chủ ban đầu, các kỹ năng tấn công vẫn chưa bị sát thương - có các gói đòn đánh riêng biệt.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 13 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x1E | Tiêu đề gói - mã định danh loại gói |
| 3 | 2 | ShortBigEndian |  | ID kỹ năng |
| 5 | 1 | Byte |  | Mục tiêuX |
| 6 | 1 | Byte |  | Mục tiêuY |
| 7 | 1 | Byte |  | Xoay |
| 10 | 2 | ShortBigEndian |  | Id mục tiêu bổ sung |
| 12 | 1 | Byte |  | Hoạt hìnhQuầy; Bộ đếm hoạt ảnh hoạt động như một tham chiếu đến gói Hoạt ảnh kỹ năng khu vực đã gửi trước đó. |