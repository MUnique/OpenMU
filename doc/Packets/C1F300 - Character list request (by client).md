# C1 F3 00 - Character list request (by client) #

## Is sent when ##
- after the client has logged in successfully. 
- when the client has left the game with a character, using the function 'Back to character selection'.


## Causes the following actions on the server side ##
If the player is authenticated, the player list is sent back to the client with the [character list response](<C1F300 - Character list response (by server).md>).


## Structure ##

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1   | [Packet type](PacketTypes.md) |
| 1 | byte | 0x04   | Packet header - length of the packet |
| 1 | byte | 0xF3   | Packet header - packet type identifier |
| 1 | byte | 0x00   | Sub packet header for character list request |
