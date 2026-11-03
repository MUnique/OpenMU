# C1 B1 00 - MapServerChangeInfo (by server)

## Is sent when

When the player has to change to another game server, e.g. for a map which is hosted on another server.

## Causes the following actions on the client side

The client connects to the given server and authenticates with ServerChangeAuthentication (B1 01). Port 0 means the change failed.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   40   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB1  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x00  | Packet header - sub packet type identifier |
| 4 | 16 | String |  | IpAddress |
| 20 | 2 | ShortLittleEndian |  | Port |
| 22 | 2 | ShortLittleEndian |  | ServerGroup |
| 24 | 4 | IntegerLittleEndian |  | AuthenticationCode1 |
| 28 | 4 | IntegerLittleEndian |  | AuthenticationCode2 |
| 32 | 4 | IntegerLittleEndian |  | AuthenticationCode3 |
| 36 | 4 | IntegerLittleEndian |  | AuthenticationCode4 |