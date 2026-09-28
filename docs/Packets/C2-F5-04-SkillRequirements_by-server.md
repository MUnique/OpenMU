# C2 F5 04 - SkillRequirements (by server)

## Is sent when

Once after a successful login, before the character selection.

## Causes the following actions on the client side

The client uses these requirements and costs for the rest of the session, instead of the ones of its own data files, so that it shows and checks exactly what the server checks.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0xF5  | Packet header - packet type identifier |
| 4 | 1 |    Byte   | 0x04  | Packet header - sub packet type identifier |
| 6 | 2 | ShortLittleEndian |  | SkillCount |
| 8 | SkillRequirement.Length * SkillCount | Array of SkillRequirement |  | Skills; The requirements of every skill of the game configuration. |

### SkillRequirement Structure

The requirements and costs of a skill, as the server checks them. A value of 0 means there is no such requirement.

Length: 16 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortLittleEndian |  | SkillNumber |
| 2 | 2 | ShortLittleEndian |  | Level; The required character level. |
| 4 | 2 | ShortLittleEndian |  | Energy; The required total energy. |
| 6 | 2 | ShortLittleEndian |  | Leadership; The required total leadership (command). |
| 8 | 2 | ShortLittleEndian |  | Strength; The required total strength. |
| 10 | 2 | ShortLittleEndian |  | Agility; The required total agility. |
| 12 | 2 | ShortLittleEndian |  | Mana; The mana which is consumed by using the skill. |
| 14 | 2 | ShortLittleEndian |  | AbilityGauge; The ability (AG) which is consumed by using the skill. |