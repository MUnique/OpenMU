# C1 B7 04 - WeaponExplosionRequest (client gửi)

## Được gửi khi nào

Sau khi người chơi bắn một máy phóng và bắn trúng một máy phóng khác.

## Hành động phía server

Máy chủ làm hỏng máy phóng khác.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 6 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xB7 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x04 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 2 | ShortBigEndian |  | Id máy phóng |