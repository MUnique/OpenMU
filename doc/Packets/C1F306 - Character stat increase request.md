# C1 F3 06 - Character stat increase request #

## Is sent when ##
When the player is has entered the game world and wants to increase a stat attribute with previously gained level up points.


## Causes the following actions on the server side ##
The server checks if the player has a selected character, available level up points, and the requested stat attribute.
If this requirements are met, the requested stat point is increased by one, and the avaible level up points decreased by one as well.
The server sends a [response](<C1F306 - Character stat increase response.md>) back to the client.

## Structure ##

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1   | [Packet type](PacketTypes.md) |
| 1 | byte | 0x05   | Packet header - length of the packet |
| 1 | byte | 0xF3   | Packet header - packet type identifier |
| 1 | byte | 0x06   | Packet header - character stat increase |
| 1 | byte |        | Stat attribute identifier |

### Stat attribute Identifier ###
| Value | Description |
|-------|-------------|
| 0x00  | Strength    |
| 0x01  | Agility     |
| 0x02  | Vitality    |
| 0x03  | Energy      |
| 0x04  | Leadership  |
