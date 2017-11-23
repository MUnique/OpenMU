# C1 F3 01 00 - Character creation failed #

## Is sent when ##
The creation of a character failed.


## Causes the following actions on the client side ##
It shows a message to the player, that the creation of the character failed.


## Structure ##

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1   | [Packet type](PacketTypes.md) |
| 1 | byte | 0x05   | Packet header - length of the packet |
| 1 | byte | 0xF3   | Packet header - packet type identifier |
| 1 | byte | 0x01   | Packet header - character creation |
| 1 | byte | 0x00   | Creation flag (failed = 0) |