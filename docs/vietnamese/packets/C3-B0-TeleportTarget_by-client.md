# C3 B0 - TeleportTarget (client gửi)

## Được gửi khi nào

Một thuật sĩ sử dụng kỹ năng 'Dịch chuyển đồng minh' để dịch chuyển một thành viên trong nhóm trong tầm nhìn của anh ta đến tọa độ gần đó.

## Hành động phía server

Nếu người chơi mục tiêu ở cùng nhóm và trong phạm vi, nó sẽ dịch chuyển đến tọa độ đã chỉ định.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 7 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xB0 | Tiêu đề gói - mã định danh loại gói |
| 3 | 2 | ShortLittleEndian |  | Id mục tiêu |
| 5 | 1 | Byte |  | Dịch chuyển mục tiêuX |
| 6 | 1 | Byte |  | Dịch chuyển mục tiêuY |