# C1 F3 15 - Character focus confirmation #

## Is sent when ##
The client [requested](<C1F315 - Character focus request.md>) to focus one of his characters.


## Causes the following actions on the client side ##
The focused character gets highlighted in the character selection screen.


## Structure ##

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1   | [Packet type](PacketTypes.md) |
| 1 | byte | 0x0F   | Packet header - length of the packet |
| 1 | byte | 0xF3   | Packet header - packet type identifier |
| 1 | byte | 0x15   | Packet header - character focus |
| 10 | string |     | Character name |
| 1 | byte | 0x00 | unused |
