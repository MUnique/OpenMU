# С2 F4 - Server list response (by server) #

## Is sent when ##

The client requested it with the server list request.

## Causes the following actions on the [client or server] side ##

Retrive current available servers from connect server with load info.

## Structure ##

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC2   | [Packet type](PacketTypes.md) |
| 2 | byte | [Length] | Packet header - length of the packet |
| 1 | byte | 0xF4   | Packet header - packet type identifier |
| 1 | byte | 0x06 (0x02 for old)   | Packet header - packet sub type identifier for "servers" |
| 2 | byte | 0xXXXX   | Servers count 0 - 65 536 |
| 4(2 old) * n | ServerInfo |  | One block of 4 bytes (2 bytes in old seasons) per server |

## ServerInfo Structure ##
|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 4 | byte | 0xXXXX   | Server load (current relation max online players vs current online) |
