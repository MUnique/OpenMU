# C1 F6 10 - Client Action Completed

## Is sent when
The client completed the an action (e.g. the tutorial).


## Causes the following actions on the server side
The server checks if the specified quest is currently in progress.
If the quest got a Condition (condition type 0x10) for this flag,
the condition is flagged as fulfilled.


## Structure

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1   | [Packet type](PacketTypes.md) |
| 1 | byte |        | Packet header - length of the packet |
| 1 | byte | 0xF6   | Packet header - packet type identifier |
| 1 | byte | 0x10   | Packet header - packet type identifier |
| 2 | short |       | Quest Number (big endian) |
| 2 | short |       | Quest Group (big endian) |
