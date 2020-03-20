# C1 11 - HitRequest (by client)

## Is sent when

A player attacks a target without using a skill.

## Causes the following actions on the server side

Damage is calculated and the target is hit, if the attack was successful. A response is sent back with the caused damage, and all surrounding players get an animation message.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x11  | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | TargetId |
| 5 | 1 | Byte |  | AttackAnimation |
| 6 | 1 | Byte |  | LookingDirection |