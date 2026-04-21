# C1 19 - SkillAnimation075 (server gửi)

## Được gửi khi nào

Một đối tượng thực hiện skill có mục tiêu trực tiếp là một đối tượng khác.

## Hành động phía client

Animation được hiển thị trên giao diện.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 8 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0x19 | Packet header - packet type identifier |
| 3 | 1 | Byte |  | SkillId |
| 4 | 2 | ShortBigEndian |  | PlayerId |
| 6 | 2 | ShortBigEndian |  | TargetId |
| 6 << 7 | 1 bit | Boolean |  | EffectApplied |
