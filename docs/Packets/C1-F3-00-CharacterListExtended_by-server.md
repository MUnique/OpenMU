# C1 F3 00 - CharacterListExtended (by server)

## Is sent when

After the game client requested it, usually after a successful login.

## Causes the following actions on the client side

The game client shows the available characters of the account.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x00  | Packet header - sub packet type identifier |
| 4 | 1 | CharacterCreationUnlockFlags |  | UnlockFlags |
| 5 | 1 | Byte |  | MoveCnt |
| 6 | 1 | Byte |  | CharacterCount |
| 7 | 1 | Boolean |  | IsVaultExtended |
| 8 | CharacterData.Length * CharacterCount | Array of CharacterData |  | Characters |

### CharacterData Structure

Data of one character in the list.

Length: 44 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte |  | SlotIndex |
| 1 | 10 | String |  | Name |
| 12 | 2 | ShortLittleEndian |  | Level |
| 14 | 4 bit | CharacterStatus |  | Status |
| 14 << 4 | 1 bit | Boolean |  | IsItemBlockActive |
| 15 | 27 | Binary |  | Appearance |
| 42 | 1 | GuildMemberRole |  | GuildPosition |

### CharacterStatus Enum

The status of a character.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Normal | The state of the character is normal. |
| 1 | Banned | The character is banned from the game. |
| 32 | GameMaster | The character is a game master. |

### GuildMemberRole Enum

Defines the role of a guild member.

| Value | Name | Description |
|-------|------|-------------|
| 0 | NormalMember | The member is a normal member without special rights. |
| 32 | BattleMaster | The member is a battle master. |
| 128 | GuildMaster | The member is the guild master. |
| 255 | Undefined | The character is not a member, therefore the role is undefined. |

### CharacterCreationUnlockFlags Enum

The flags to unlock the specified character classes for the creation of new characters.

| Value | Name | Description |
|-------|------|-------------|
| 0 | None | No unlocked class. |
| 1 | Summoner | Unlocks the summoner class. |
| 2 | DarkLord | Unlocks the dark lord class. |
| 4 | MagicGladiator | Unlocks the magic gladiator class. |
| 8 | RageFighter | Unlocks the rage fighter class. |