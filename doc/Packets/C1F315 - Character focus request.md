# C1 F3 15 - Character focus request #

## Is sent when ##
When the player is in the character selection screen and focuses an existing character of his account.


## Causes the following actions on the server side ##
The server checks if the player is in the character selection state. 
If it does, it checks if the character exists in the account.
If this requirements are met, the server sends a [confirmation](<C1F315 - Character focus confirmation.md>) of the focus back to the client.

## Structure ##

|  Length  | Data type | Value | Description |
|----------|---------|-------------|
| 1 | byte | 0xC1   | [Packet type](PacketTypes.md) |
| 1 | byte | 0x0E   | Packet header - length of the packet |
| 1 | byte | 0xF3   | Packet header - packet type identifier |
| 1 | byte | 0x15   | Packet header - character focus |
| 10 | string |     | Name of the character |
