# C1 15 - ObjectMoved (server gửi)

## Được gửi khi nào

Một vật thể trong phạm vi quan sát (bao gồm cả người chơi của chính mình) sẽ di chuyển ngay lập tức.

## Hành động phía client

Vị trí của đối tượng được cập nhật ở phía máy khách.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 8 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x15 | Tiêu đề gói - mã định danh loại gói |
| 2 | 1 | Byte |  | Mã tiêu đề |
| 3 | 2 | ShortBigEndian |  | ID đối tượng |
| 5 | 1 | Byte |  | Vị tríX |
| 6 | 1 | Byte |  | Vị tríY |