# C1 F3 30 - Save character key configuration #

## Is sent when ##
When the player is about to exit the game or moves to the character selection screen.


## Causes the following actions on the server side ##
The server just takes the raw payload and puts it into the characters KeyConfiguration property.
When the player exits the game, it's then saved to the database.

## Structure ##

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1   | [Packet type](PacketTypes.md) |
| 1 | byte | 0x22   | Packet header - length of the packet |
| 1 | byte | 0xF3   | Packet header - packet type identifier |
| 1 | byte | 0x30   | Packet header - save character key configuration |
| 32 | byte[] |     | payload, byte array with the key configuration |

The key configuration basically contains the skill id (each 2 bytes) of each key from 0 to 9, the
potion/consumable item types for keys (Q, W, E, R) and maybe some other key configuration settings which are saved per character.