# C1 BD 08 - CrywolfHeroList (by server)

## Is sent when

The crywolf event ended, after the CrywolfInfo with the state End.

## Causes the following actions on the client side

The client shows the heroes with the highest scores in the result.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBD  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x08  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | HeroCount; The number of heroes, at most 5. |
| 5 | Hero.Length * HeroCount | Array of Hero |  | Heroes; The heroes. |

### Hero Structure

Contains a hero of the crywolf event.

Length: 20 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte |  | Rank; The place of the hero, from 0 (first) to 4. The client uses it as array index, so it must not be greater than 4. |
| 1 | 10 | String |  | Name; The name of the hero. |
| 12 | 4 | IntegerLittleEndian |  | Score; The score of the hero. The field is aligned to 4 bytes, because the client structure is not packed. |
| 16 | 1 | Byte |  | CharacterClass; The number of the character class. |