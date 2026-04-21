# C3 DB - AreaSkillHit (client gửi)

## Được gửi khi nào

Một kỹ năng khu vực đã được thực hiện và khách hàng quyết định bắn trúng mục tiêu.

## Hành động phía server

Máy chủ đang tính toán sát thương và áp dụng nó lên mục tiêu. Kẻ tấn công nhận được phản hồi lại với sát thương gây ra.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte |  | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xDB | Tiêu đề gói - mã định danh loại gói |
| 3 | 2 | ShortBigEndian |  | ID kỹ năng |
| 5 | 1 | Byte |  | Mục tiêuX |
| 6 | 1 | Byte |  | Mục tiêuY |
| 7 | 1 | Byte |  | Lượt truy cập; Bộ đếm lần truy cập tuần tự sẽ ngăn chặn các lần truy cập được gửi nhiều lần. |
| 8 | 1 | Byte |  | Đếm mục tiêu; Số lượng mục tiêu sẽ tuân theo trong cấu trúc. |
| 9 | TargetData.Length * TargetCount | Array of TargetData |  | Mục tiêu |

### Cấu trúc TargetData
Chứa dữ liệu của mục tiêu

Độ dài: 3 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortBigEndian |  | Id mục tiêu |
| 2 | 1 | Byte |  | Hoạt hìnhQuầy; Bộ đếm hoạt ảnh tuần tự hoạt động như một tham chiếu đến gói Hoạt ảnh kỹ năng khu vực đã gửi trước đó. |