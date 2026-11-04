# C1 A8 - PetAttack (by server)

## Is sent when

After the client sent a PetAttackCommand, the pet attacks automatically. For each attack, the player and all observing players get this message.

## Causes the following actions on the client side

The client shows the pet attacking the target.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   9   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xA8  | Packet header - packet type identifier |
| 3 | 1 | ClientToServer.PetType | ClientToServer.PetType.DarkRaven | Pet |
| 4 | 1 | PetSkillType |  | SkillType |
| 5 | 2 | ShortBigEndian |  | OwnerId |
| 7 | 2 | ShortBigEndian |  | TargetId |

### PetSkillType Enum

Describes the type of the pet attack.

| Value | Name | Description |
|-------|------|-------------|
| 0 | SingleTarget | A single target attack, used for critical and excellent hits. |
| 1 | Range | A range attack for multiple targets, usually up to 3 additional targets which all get their own PetAttack messages with 'SingleTarget' right after the first 'Range' message. |