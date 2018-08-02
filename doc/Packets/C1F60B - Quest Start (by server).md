# C1 F6 0B - Quest Start

## Is sent when
After a client [requested](<C1F60A - Quest Initialization Request (by client).md>) to initialize a quest.


## Causes the following actions on the client side
The client shows the quest data and state accordingly.
I guess this only shows the description of the quest in the dialog.


## Structure

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1   | [Packet type](PacketTypes.md) |
| 1 | byte | 0x0B | Packet header - length of the packet |
| 1 | byte | 0xF6   | Packet header - packet type identifier |
| 1 | byte | 0x0B   | Packet header - packet type identifier |
| 2 | short |        | Quest Number (big endian) |
| 2 | short |        | Quest Group (big endian) |
| 3 | byte |        | Unknown, maybe quest states? |
