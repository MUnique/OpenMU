# C2 F5 05 - LearnableItemRequirements (by server)

## Is sent when

Once after a successful login, right after the SkillRequirements message.

## Causes the following actions on the client side

The client shows these requirements for the items which teach a skill (orbs, scrolls, parchments, crystals), instead of the ones it calculates from its own data files.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0xF5  | Packet header - packet type identifier |
| 4 | 1 |    Byte   | 0x05  | Packet header - sub packet type identifier |
| 6 | 2 | ShortLittleEndian |  | ItemCount |
| 8 | LearnableItemRequirement.Length * ItemCount | Array of LearnableItemRequirement |  | Items; The requirements to learn the skill of each item. |

### LearnableItemRequirement Structure

The requirements to learn a skill with an item: the highest of the requirements of the item and the requirements of the skill. A value of 0 means there is no such requirement.

Length: 16 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte |  | Group |
| 1 | 1 | Byte |  | ItemLevel; The item level these requirements are for, or 0xFF if they apply to every level. Items which teach a different skill per level (Orb of Summoning) have one entry per level. |
| 2 | 2 | ShortLittleEndian |  | Number |
| 4 | 2 | ShortLittleEndian |  | Level; The required character level. |
| 6 | 2 | ShortLittleEndian |  | Energy; The required total energy. |
| 8 | 2 | ShortLittleEndian |  | Leadership; The required total leadership (command). |
| 10 | 2 | ShortLittleEndian |  | Strength; The required total strength. |
| 12 | 2 | ShortLittleEndian |  | Agility; The required total agility. |
| 14 | 2 | ShortLittleEndian |  | SkillNumber; The skill which is learned with the item. |