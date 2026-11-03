# C1 F3 16 - CharacterRenameResult (by server)

## Is sent when

After the player requested to rename a character.

## Causes the following actions on the client side

If Result is 0, the client renames the character to NewName; otherwise it shows an error.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   25   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x16  | Packet header - sub packet type identifier |
| 4 | 10 | String |  | OldName |
| 14 | 10 | String |  | NewName |
| 24 | 1 | Byte |  | Result |