# C1 15 - InstantMoveRequest (client gửi)

## Được gửi khi nào

Nó được gửi khi người chơi thực hiện các kỹ năng cụ thể.

## Hành động phía server

Thông thường, người chơi sẽ được di chuyển ngay lập tức đến tọa độ đã chỉ định trên bản đồ hiện tại. Trong OpenMU, yêu cầu này không được xử lý, vì nó cho phép hacker “dịch chuyển” tới bất kỳ tọa độ nào.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x15 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte |  | Mục tiêuX |
| 4 | 1 | Byte |  | Mục tiêuY |