# C1 1B - MagicEffectCancelRequest (by client)

## Is sent when

A player cancels a specific magic effect of a skill, usually 'Infinity Arrow' and 'Wizardy Enhance'.

## Causes the following actions on the server side

The effect is cancelled and an update is sent to the player and all surrounding players.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   7   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x1B  | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | SkillId |
| 5 | 2 | ShortBigEndian |  | PlayerId |