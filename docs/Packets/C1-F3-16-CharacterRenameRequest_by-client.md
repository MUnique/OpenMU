# C1 F3 16 - CharacterRenameRequest (by client)

## Is sent when

The player wants to rename a character in the character selection.

## Causes the following actions on the server side

The server renames the character and answers with CharacterRenameResult (F3 16).

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   24   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x16  | Packet header - sub packet type identifier |
| 4 | 10 | String |  | OldName |
| 14 | 10 | String |  | NewName |