# C1 11 - HitRequest (client gửi)

## Được gửi khi nào

Khi người chơi tấn công mục tiêu mà không dùng skill.

## Hành động phía server

Server tính toán damage và áp dụng hit lên mục tiêu nếu đòn đánh hợp lệ.
Sau đó:

- gửi response về lượng damage gây ra
- gửi message animation tới các người chơi xung quanh

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 7 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0x11 | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | TargetId |
| 5 | 1 | Byte |  | AttackAnimation |
| 6 | 1 | Byte |  | LookingDirection |
