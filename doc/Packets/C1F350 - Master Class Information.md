# C1 F3 50 - Master Class Information #

## Is sent when ##
A player enters the game with a character which has a master character class assigned.


## Causes the following actions on the client side ##
* Maximum stat values like health, mana, etc. are shown, otherwise they would stay 0.
* Additional Master related data is available
  * the master experience in the experience bar
  * the master level and available points in the skill tree dialog


## Structure ##

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1   | [Packet type](PacketTypes.md) |
| 1 | byte | 0x20 | Packet header - length of the packet |
| 1 | byte | 0xF3   | Packet header - packet type identifier |
| 1 | byte | 0x50   | Packet header - packet subtype identifier |
| 2 | short |       | Master Level of the Character (big endian) |
| 8 | long  |       | The current master experience of the Character (small endian) |
| 8 | long  |       | The required master experience for the next master level (small endian) |
| 2 | short |       | Available master points (big endian) |
| 2 | short |       | Maximum Health of the Character (big endian) |
| 2 | short |       | Maximum Mana of the Character (big endian) |
| 2 | short |       | Maximum Shield of the Character (big endian) |
| 2 | short |       | Maximum Ability of the Character (big endian) |