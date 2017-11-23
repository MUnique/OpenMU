# C1 F3 03 - Character select request #

## Is sent when ##
When the player is in the character selection screen and selects an existing character of his account to enter the game world.


## Causes the following actions on the server side ##
The server checks if the player is in the character selection state. 
If it does, it checks if the character exists in the account.
If this requirements are met, the player enters the world with the selected character.
The server initializes the player object and sends then the following data back to the client:

- Skill List
- Detailed character information
- Inventory
- Own player <> guild binding
- Player rotation

Additionally the world entrance of the character is registered in the guild und friend server.

## Structure ##

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1   | [Packet type](PacketTypes.md) |
| 1 | byte | 0x0D   | Packet header - length of the packet |
| 1 | byte | 0xF3   | Packet header - packet type identifier |
| 1 | byte | 0x03   | Packet header - character selection |
| 10 | string |     | Name of the character |
