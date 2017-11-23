# C1 F3 02 - Character delete request #

## Is sent when ##
When the player is in the character selection screen he can choose to delete an existing character. This request is sent, when he tries to do that.


## Causes the following actions on the server side ##
The server checks if the player is in the character selection state. 
If it does, it checks if the character exists in the account and if the security code is correct.
If this requirements are met, the character gets deleted from the account.
In any case, the server sends a [character delete response](C1F302 - Character delete response.md) with a result code.

## Structure ##

|  Length  | Data type | Value | Description |
|----------|---------|-------------|
| 1 | byte | 0xC1   | [Packet type](PacketTypes.md) |
| 1 | byte | 0x15   | Packet header - length of the packet |
| 1 | byte | 0xF3   | Packet header - packet type identifier |
| 1 | byte | 0x02   | Packet header - character deletion |
| 10 | string |     | Name of the character |
| 7 | string  |     | Account security code |
