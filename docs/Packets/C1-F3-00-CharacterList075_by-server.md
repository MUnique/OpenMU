# C1 F3 00 - CharacterList075 (by server)

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
| 4 | 1 | Byte |  | CharacterCount |
| 5 | CharacterData.Length * CharacterCount | Array of CharacterData |  | Characters |

### CharacterData Structure

Data of one character in the list.

Length: 24 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte |  | SlotIndex |
| 1 | 10 | String |  | Name |
| 11 | 2 | ShortLittleEndian |  | Level |
| 13 | 4 bit | CharacterStatus |  | Status |
| 13 << 4 | 1 bit | Boolean |  | IsItemBlockActive |
| 14 | 9 | Binary |  | Appearance |

### CharacterStatus Enum

The status of a character.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Normal | The state of the character is normal. |
| 1 | Banned | The character is banned from the game. |
| 32 | GameMaster | The character is a game master. |