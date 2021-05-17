# C1 A7 - PetCommandRequest (by client)

## Is sent when

The player wants to command its equipped pet (raven).

## Causes the following actions on the server side



## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   7   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xA7  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | PetType |
| 4 | 1 | PetCommandMode |  | CommandMode |
| 5 | 2 | ShortBigEndian |  | TargetId |

### PetCommandMode Enum

Describes the pet command mode.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Normal | The pet is in a normal mode, where it doesn't attack. |
| 1 | AttackRandom | The pet attacks random targets. |
| 2 | AttackWithOwner | The pet attacks the same targets as the owner. |
| 3 | AttackTarget | The pet attacks a specific target until it's dead. |