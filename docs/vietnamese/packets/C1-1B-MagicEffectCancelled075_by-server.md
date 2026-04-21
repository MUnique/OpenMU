# C1 1B - MagicEffectCancelled075 (server gửi)

## Được gửi khi nào

Người chơi đã huỷ một hiệu ứng ma thuật cụ thể của skill (Infinity Arrow, Wizardry Enhance), hoặc hiệu ứng bị gỡ do hết thời gian (Ice, Poison) hoặc thuốc giải.

## Hành động phía client

Hiệu ứng bị gỡ khỏi đối tượng mục tiêu.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 6 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0x1B | Packet header - packet type identifier |
| 3 | 1 | Byte |  | SkillId |
| 4 | 2 | ShortBigEndian |  | TargetId |
