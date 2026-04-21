# C1 11 - ObjectHit (server gửi)

## Được gửi khi nào

Object nhận hit trong 2 trường hợp:

1. Người chơi hiện tại bị đánh trúng.
2. Người chơi hiện tại đánh trúng object khác.

## Hành động phía client

Hiển thị damage trên object nhận đòn đánh.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 10 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0x11 | Packet header - packet type identifier |
| 2 | 1 | Byte |  | HeaderCode |
| 3 | 2 | ShortBigEndian |  | ObjectId |
| 5 | 2 | ShortBigEndian |  | HealthDamage |
| 7 << 0 | 4 bit | DamageKind |  | Kind |
| 7 << 4 | 1 bit | Boolean |  | IsRageFighterStreakHit |
| 7 << 5 | 1 bit | Boolean |  | IsRageFighterStreakFinalHit |
| 7 << 6 | 1 bit | Boolean |  | IsDoubleDamage |
| 7 << 7 | 1 bit | Boolean |  | IsTripleDamage |
| 8 | 2 | ShortBigEndian |  | ShieldDamage |

### Enum DamageKind

Định nghĩa loại damage hiển thị theo màu sắc.

| Value | Name | Description |
|-------|------|-------------|
| 0 | NormalRed | Màu đỏ, damage thường. |
| 1 | IgnoreDefenseCyan | Màu cyan, thường cho ignore defense. |
| 2 | ExcellentLightGreen | Màu xanh lá nhạt, thường cho excellent damage. |
| 3 | CriticalBlue | Màu xanh dương, thường cho critical damage. |
| 4 | ReflectedLightPink | Màu hồng nhạt, thường cho phản damage. |
| 5 | PoisonDarkGreen | Màu xanh đậm, thường cho poison damage. |
| 6 | DarkPink | Màu hồng đậm. |
| 7 | White | Màu trắng. |
