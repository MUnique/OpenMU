# C1 A0 - Request Quest State

## Is sent when ##
Unknown.


## Causes the following actions on the server side ##
The server sends the current [quest state](<C1A0 - Quest state response (by server).md>) as response.


## Structure ##

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1    | [Packet type](PacketTypes.md) |
| 1 | byte | 0x03    | Packet header - length of the packet |
| 1 | byte | 0xA0    | Packet header - packet type identifier |
