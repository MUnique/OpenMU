# C1 F6 1B - Quest State Response

## Is sent when
After the client [requested](<C1F61B - Quest State Request (by client).md>) it.

## Causes the following actions on the client side
The client shows the quest progress accordingly.


## Structure

The structure is equal to the [quest progress](<C1F60C - Quest Progress (by server).md>), just with a different header. More details can be found there.

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1   | [Packet type](PacketTypes.md) |
| 1 | byte |        | Packet header - length of the packet |
| 1 | byte | 0xF6   | Packet header - packet type identifier |
| 1 | byte | 0x1B   | Packet header - packet type identifier |
| 2 | short |       | Quest Number (big endian) |
| 2 | short |       | Quest Group (big endian) |
| 1 | byte |        | Number of Conditions in the following array |
| 1 | byte |        | Number of Rewards in the following array |
| 1 | byte |        | padding |
| 5 | Condition[] | | Quest completion conditions, 5 times  |
| 5 | Reward[]|     | Quest completion reward, 5 times |