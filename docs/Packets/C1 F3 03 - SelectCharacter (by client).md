# C1 F3 03 - SelectCharacter (by client)

## Is sent when

The player selects a character to enter the game world on the character selection screen.

## Causes the following actions on the server side

The player joins the game world with the specified character.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x03  | Packet header - sub packet type identifier |
| 4 | 10 | String |  | Name; The name of the character with which the player wants to join the game world |