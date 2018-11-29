# C1 F3 06 - Character stat increase response #

## Is sent when ##
The client [requested](<C1F306 - Character stat increase request.md>) to increase a stat attribute of his currently selected character.


## Causes the following actions on the client side ##
The value of the stat attribute gets increased by one if the success flag is 1.


## Structure ##

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1   | [Packet type](PacketTypes.md) |
| 1 | byte | 0x0C   | Packet header - length of the packet |
| 1 | byte | 0xF3   | Packet header - packet type identifier |
| 1 | byte | 0x06   | Packet header - character stat increase |
| 1 | bit |  | Success flag |
| 3 | bit |  | unused |
| 4 | bit |  | Stat attribute identifier |
| 1 | byte |        | Padding |
| 1 | ushort (BE) |        | If successful: Depends on stat attribute identifier; For energy, it's the new maximum mana. For vitality it's the new maximum health. |
| 1 | ushort (BE) |        | If successful: New maximum shield |
| 1 | ushort (BE) |        | If successful: New maximum ability |

### Stat attribute Identifier ###
| Value | Description |
|-------|-------------|
| 0x00  | Strength    |
| 0x01  | Agility     |
| 0x02  | Vitality    |
| 0x03  | Energy      |
| 0x04  | Leadership  |
