# C1 F3 15 - FocusCharacter (by client)

## Is sent when

The player focuses (clicks on it) a character with which he plans to enter the game world on the character selection screen.

## Causes the following actions on the server side

The server checks if this character exists and sends a response back. If successful, the game client highlights the focused character.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x15  | Packet header - sub packet type identifier |
| 4 | 10 | String |  | Name |