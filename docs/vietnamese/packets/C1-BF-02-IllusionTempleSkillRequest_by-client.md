# C1 BF 02 - IllusionTempleSkillRequest (client gửi)

## Được gửi khi nào

Người chơi đang tham gia sự kiện ngôi đền ảo ảnh và muốn thực hiện một kỹ năng đặc biệt (210 - 213), Lệnh Bảo vệ, Kiềm chế, Theo dõi hoặc Làm suy yếu.

## Hành động phía server

Máy chủ kiểm tra xem người chơi có tham gia sự kiện hay không, v.v. và thực hiện các kỹ năng tương ứng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 8 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xBF | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x02 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 2 | ShortBigEndian |  | Số kỹ năng |
| 6 | 1 | Byte |  | Chỉ mục đối tượng mục tiêu |
| 7 | 1 | Byte |  | Khoảng cách |