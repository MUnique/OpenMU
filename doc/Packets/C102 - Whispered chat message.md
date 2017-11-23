# C1 02 - Whispered chat message #

## Is sent when ##
A player sends a whispered chat message to a named recipient.


## Causes the following actions on the server side ##
The message is forwarded to the named player, if the player is online at the same sub-server.

## Causes the following actions on the client side ##
The message is displayed in the chat message area.

## Structure ##

|  Length  | Data type | Value | Description |
|----------|---------|-------------|
| 1 | byte | 0xC1   | [Packet type](PacketTypes.md) |
| 1 | byte | [Length] | Packet header - length of the packet |
| 1 | byte | 0x02   | Packet header - packet type identifier |
| 10| string |     | Recipient player name string, terminated by 0x00 if shorter than 10 |
| n | string |     | The message, terminated by 0x00 if shorter than n |
