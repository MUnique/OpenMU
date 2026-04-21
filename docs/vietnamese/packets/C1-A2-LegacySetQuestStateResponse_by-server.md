# C1 A2 - LegacySetQuestStateResponse (server gửi)

## Được gửi khi nào

Để đáp ứng yêu cầu trạng thái đã đặt (C1A2).

## Hành động phía client

Ứng dụng khách trò chơi hiển thị trạng thái nhiệm vụ mới.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 6 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xA2 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte |  | Chỉ mục nhiệm vụ |
| 4 | 1 | Byte |  | Kết quả; Giá trị này là 0 nếu thành công. Mặt khác, 0xFF hoặc thậm chí các giá trị ma thuật khác. |
| 5 | 1 | Byte |  | Bang mới; Đây là byte hoàn chỉnh với trạng thái của bốn nhiệm vụ trong cùng một byte. |