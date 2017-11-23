# C1 F3 00 - Character list response (by server) #

## Is sent when ##
The client requested it with the [character list request](<C1F300 - Character list request (by client).md>) and the client is authenticated.


## Causes the following actions on the client side ##
The character list is shown. The client has then the following functions available:

- select the character the player wants to play with
- create a new character
- delete a character


## Structure ##

|  Length  | Data type | Value | Description |
|----------|---------|-------------|
| 1 | byte | 0xC1   | [Packet type](PacketTypes.md) |
| 1 | byte | [Length] | Packet header - length of the packet |
| 1 | byte | 0xF3   | Packet header - packet type identifier |
| 1 | byte | 0x00   | Sub packet header for character list |
| 1 | byte |    | A flag which indicates the unlocked character classes |
| 1 | byte | [MoveCnt] | TODO |
| 1 | byte | n | Number of characters |
| 1 | byte |  | Number of vault extensions |
| 34 * n | CharacterInfo | | One block of 34 bytes per character |

### Character Information Block Structure ###
|  Length  | Data type | Value | Description |
|----------|---------|-------------|
| 1 | byte | 1 ~ 5 | Number of the character slot |
| 10 | string | | Name of the character, terminated by 0x00, if shorter than 10 |
| 1 | byte | 1 | unknown |
| 2 | short (BE) | | Character level |
| 1 | byte | | State code |
| 18 | [Appearance](Appearance.md) | | The [appearance data](Appearance.md) of the character |
| 1 | byte | | Guild member status code |