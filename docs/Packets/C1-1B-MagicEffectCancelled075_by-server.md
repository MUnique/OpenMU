# C1 1B - MagicEffectCancelled075 (by server)

## Is sent when

A player cancelled a specific magic effect of a skill (Infinity Arrow, Wizardry Enhance), or an effect was removed due a timeout (Ice, Poison) or antidote.

## Causes the following actions on the client side

The effect is removed from the target object.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x1B  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | SkillId |
| 4 | 2 | ShortBigEndian |  | TargetId |