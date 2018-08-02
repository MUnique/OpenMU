# C1 F6 03 - Quest Event Response

## Is sent when
The client [requested](<C1F621 - Quest Event Request (by client).md>) it after entering the game.
It seems to be sent only if the character is not a member of a Gen.

## Causes the following actions on the client side
Unknown.


## Structure

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1   | [Packet type](PacketTypes.md) |
| 1 | byte | 0x0C   | Packet header - length of the packet |
| 1 | byte | 0xF6   | Packet header - packet type identifier |
| 1 | byte | 0x03   | Packet header - packet type identifier |
| 2 | short |    | Quest #1 Number |
| 2 | short |    | Quest #1 Group |
| 2 | short |    | Quest #2 Number |
| 2 | short |    | Quest #2 Group |

These two quest blocks are probably there for the two possible Gens - Duprian and Vanert. However, in my captured example packets (on GMO years ago!), these two blocks were identical, so we don't know the real meaning.

## Example
```
C1 0C F6 03 // Header
00 00 01 00 // Block for Duprian or Vanert?
00 00 01 00 // Block for Duprian or Vanert?
```
