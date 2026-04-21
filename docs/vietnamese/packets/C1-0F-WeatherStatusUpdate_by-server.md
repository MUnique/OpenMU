# C1 0F - WeatherStatusUpdate (server gửi)

## Được gửi khi nào

Thời tiết trên bản đồ hiện tại đã bị thay đổi hoặc người chơi đã vào bản đồ.

## Hành động phía client

Ứng dụng khách trò chơi cập nhật các hiệu ứng thời tiết.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x0F | Tiêu đề gói - mã định danh loại gói |
| 3 | 4 bit | Byte |  | Thời tiết; Một giá trị ngẫu nhiên trong khoảng từ 0 đến 2 (bao gồm). |
| 3 | 4 bit | Byte |  | Biến thể; Một giá trị ngẫu nhiên trong khoảng từ 0 đến 9 (đã bao gồm). |