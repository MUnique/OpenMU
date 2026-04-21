# C1 19 - TargetedSkill075 (client gửi)

## Được gửi khi nào

Khi người chơi thực hiện một skill có mục tiêu, ví dụ tấn công hoặc buff.

## Hành động phía server

Sát thương được tính toán và mục tiêu bị trúng đòn nếu tấn công thành công. Server gửi response với sát thương gây ra, và các người chơi xung quanh nhận packet animation.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 6 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0x19 | Packet header - packet type identifier |
| 3 | 1 | Byte |  | SkillIndex; Chỉ số skill trong danh sách skill. |
| 4 | 2 | ShortBigEndian |  | TargetId |
