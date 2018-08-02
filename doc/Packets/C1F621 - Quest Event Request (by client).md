# C1 F6 21 - Quest Event Request

## Is sent when
The client entered the game.

## Causes the following actions on the server side
The server may answer with a response which seems to depend if the character is member of a Gen or not.
If it's not in a gen, it sends a [response](<C1F603 - Quest Event Response (by server).md>).


## Structure

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1   | [Packet type](PacketTypes.md) |
| 1 | byte | 0x04   | Packet header - length of the packet |
| 1 | byte | 0xF6   | Packet header - packet type identifier |
| 1 | byte | 0x21   | Packet header - packet type identifier |
