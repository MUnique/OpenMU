# C1 DE 00 - CharacterClassCreationUnlock (by server)

## Is sent when

It's send right after the CharacterList, in the character selection screen, if the account has any unlocked character classes.

## Causes the following actions on the client side

The client unlocks the specified character classes, so they can be created.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xDE  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x00  | Packet header - sub packet type identifier |
| 4 | 1 | CharacterCreationUnlockFlags |  | UnlockFlags |

### CharacterCreationUnlockFlags Enum

The flags to unlock the specified character classes for the creation of new characters.

| Value | Name | Description |
|-------|------|-------------|
| 0 | None | No unlocked class. |
| 1 | Summoner | Unlocks the summoner class. |
| 2 | DarkLord | Unlocks the dark lord class. |
| 4 | MagicGladiator | Unlocks the magic gladiator class. |
| 8 | RageFighter | Unlocks the rage fighter class. |