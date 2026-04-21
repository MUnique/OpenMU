# C1 F3 20 - SummonHealthUpdate (server gửi)

## Được gửi khi nào

Khi sức khỏe của quái vật được triệu hồi (Kỹ năng yêu tinh) thay đổi.

## Hành động phía client

Tình trạng sức khỏe được cập nhật trên giao diện người dùng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF3 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x20 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Byte |  | Phần trăm sức khỏe |