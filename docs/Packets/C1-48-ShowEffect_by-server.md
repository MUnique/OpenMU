# C1 48 - ShowEffect (by server)

## Is sent when

After a player achieved or lost something.

## Causes the following actions on the client side

An effect is shown for the affected player.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x48  | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | PlayerId |
| 5 | 1 | EffectType |  | Effect |

### EffectType Enum

Defines the effect which is shown for the player.

| Value | Name | Description |
|-------|------|-------------|
| 3 | ShieldPotion | The player gained shield by drinking a potion. |
| 16 | LevelUp | A level up effect is shown for the player. |
| 17 | ShieldLost | The players shield depleted. |