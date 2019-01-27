# C1 F3 52 - Master skill level increase response #

## Is sent when ##
The client [requested](C1F352 - Master skill level increase request.md) to increase or learn a master skill of his currently selected character.


## Causes the following actions on the client side ##
The value of the master skill level gets increased by one if the success flag is 1.


## Structure ##

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1   | [Packet type](PacketTypes.md) |
| 1 | byte |    | Packet header - length of the packet |
| 1 | byte | 0xF3   | Packet header - packet type identifier |
| 1 | byte | 0x52   | Packet header - master skill level increase |
| 1 | byte |  | Success flag |
| 1 | byte |  | padding |
| 2 | ushort (BE) |  | Remaining master points |
| 1 | byte |  | Master skill index |
| 3 | byte |  | padding |
| 2 | ushort (BE) | 0xA601 | Skill Number |
| 2 | byte |  | padding |
| 1 | byte |  | Updated skill level |
| 3 | byte |  | padding |
| 4 | float |  | new current value of the skill effect |
| 4 | float |  | value of the skills next level of the skill effect |

