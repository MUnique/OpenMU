# C1 F3 07 - PoisonDamage (server gửi)

## Được gửi khi nào

Nhân vật bị hư hại do bị đầu độc trên các phiên bản client cũ.

## Hành động phía client

Loại bỏ sát thương từ máu mà không hiển thị số sát thương.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 8 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF3 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x07 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 2 | ShortBigEndian |  | Sức khỏeSát thương |
| 6 | 2 | ShortBigEndian |  | CurrentShield |