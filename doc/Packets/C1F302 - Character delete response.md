# C1 F3 02 - Character delete response #

## Is sent when ##
The client requested to delete one of his characters.


## Causes the following actions on the client side ##
In the case of success, the character gets removed from the character selection screen.
In the case of failure, a message is shown to the player with the reason.


## Structure ##

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1   | [Packet type](PacketTypes.md) |
| 1 | byte | 0x05   | Packet header - length of the packet |
| 1 | byte | 0xF3   | Packet header - packet type identifier |
| 1 | byte | 0x02   | Packet header - character delete |
| 1 | byte | 0x00   | Result code (see below) |

### Result code ###

| Code | Description |
|------|-------------|
| 0x00 | Unsuccessful |
| 0x01 | Successful |
| 0x02 | Wrong security code |