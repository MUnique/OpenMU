# C1 11 - ObjectHit (by server)

## Is sent when

An object got hit in two cases: 1. When the own player is hit; 2. When the own player attacked some other object which got hit.

## Causes the following actions on the client side

The damage is shown at the object which received the hit.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   10   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x11  | Packet header - packet type identifier |
| 2 | 1 | Byte |  | HeaderCode |
| 3 | 2 | ShortBigEndian |  | ObjectId |
| 5 | 2 | ShortBigEndian |  | HealthDamage |
| 7 << 0 | 4 bit | DamageKind |  | Kind |
| 7 << 6 | 1 bit | Boolean |  | IsDoubleDamage |
| 7 << 7 | 1 bit | Boolean |  | IsTripleDamage |
| 8 | 2 | ShortBigEndian |  | ShieldDamage |

### DamageKind Enum

Defines the kind of the damage.

| Value | Name | Description |
|-------|------|-------------|
| 0 | NormalRed | Red color, used by normal damage. |
| 1 | IgnoreDefenseCyan | Cyan color, usually used by ignore defense damage. |
| 2 | ExcellentLightGreen | Light green color, usually used by excellent damage. |
| 3 | CriticalBlue | Blue color, usually used by critical damage. |
| 4 | LightPink | Light pink color. |
| 5 | PoisonDarkGreen | Dark green color, usually used by poison damage. |
| 6 | ReflectedDarkPink | Dark pink color, usually used by reflected damage. |
| 7 | White | White color. |