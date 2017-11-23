# C1 F3 01 - Character creation request #

## Is sent when ##
When the player is in the character selection screen he can choose to create a new character. This request is sent, when he tries to do that.


## Causes the following actions on the server side ##
The server checks if the player is in the character selection state. 
If it does, it checks if the requested character class can be created by the player. 
Some character classes have requirements which the player has to be fulfill before.
The server also checks if there is a free slot for the a new character.

If the creation was successful, the character will be put into the first free character slot of the account, and the [character creation success message](<C1F30101 - Character creation successful.md>) is sent to the client.

If the creation wasn't successful, the [character creation failed message](<C1F30100 - Character creation failed.md>) will be sent to the client.

## Structure ##

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1   | [Packet type](PacketTypes.md) |
| 1 | byte | 0x0F   | Packet header - length of the packet |
| 1 | byte | 0xF3   | Packet header - packet type identifier |
| 1 | byte | 0x01   | Packet header - character creation |
| 10 | string |     | Name of the new character |
| 1 | byte |        | Character class identifier |
