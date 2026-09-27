# C2 F5 06 - MonsterLevels (by server)

## Is sent when

Once after a successful login, right after the LearnableItemRequirements message.

## Causes the following actions on the client side

The client shows the level of the monsters next to their name and health bar.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0xF5  | Packet header - packet type identifier |
| 4 | 1 |    Byte   | 0x06  | Packet header - sub packet type identifier |
| 6 | 2 | ShortLittleEndian |  | MonsterCount |
| 8 | MonsterLevel.Length * MonsterCount | Array of MonsterLevel |  | Monsters; The level of each monster which has one. |

### MonsterLevel Structure

The level of a monster.

Length: 4 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortLittleEndian |  | MonsterNumber; The number of the monster, as in the AddMonstersToScope message. |
| 2 | 2 | ShortLittleEndian |  | Level |