# C1 A2 - Set quest state request

## Is sent when ##
The player starts, cancels or finishes a quest.


## Causes the following actions on the server side ##
The server sets the new quest state and sends a response.


## Structure ##

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1    | [Packet type](PacketTypes.md) |
| 1 | byte | 0x03    | Packet header - length of the packet |
| 1 | byte | 0xA2    | Packet header - packet type identifier |
| 1 | byte |         | Index of the quest |
| 1 | byte |         | New requested state of the quest |

The values of the index and state of the quest are documented [here](<C1A0 - Quest state response (by server).md>).