# C1 F3 00 - RequestCharacterList (by client)

## Is sent when

After a successful login or after the player decided to leave the game world to go back to the character selection screen.

## Causes the following actions on the server side

The server sends the character list with all available characters.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x00  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Language |