# C1 A4 - Quest kill count info

## Is sent when ##
The client talks to the NPC which gave the quest, the quest is not yet in the completed state and the quest requires to kill monsters.


## Causes the following actions on the client side ##
The client shows the current kill count of the required monster kills. The client knows required kill count by its configuration.
If the current kill count is equal to the required kill count for all monsters, the client
usually offers to complete the quest (by sending the [set quest state request message](<C1A2 - Set quest state request (by client).md>)).


## Structure ##

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1    | [Packet type](PacketTypes.md) |
| 1 | byte |         | Packet header - length of the packet |
| 1 | byte | 0xA4    | Packet header - packet type identifier |
| 1 | byte | 0x00    | Packet header - sub packet type identifier, always 0 |
| 1 | byte | 0x01    | Result, always 1 |
| 1 | byte |         | Quest index |
| 2 | byte |         | padding |
| 40 | uint[] |  | The kill counts for 1 to 5 monster types. Pairs of integers (big endian), where the first int is the monster number, and the second int is the kill count. Unused monster slots are filled up with 0xFF.|

### Example

This packet was captured before completing the last quest at the NPC.

C1 30 A4 00 01 06 00 00 **9C 01 00 00** *01 00 00 00* FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF
  * **9C 01 00 00** -> It's 412 in decimal, which is the number for dark elf
  * *01 00 00 00* -> Current kill count of 1