# C1 F4 03 - ConnectionInfo (by server)

## Is sent when

This packet is sent by the server after the client requested the connection information of a server. This happens after the user clicked on a server.

## Causes the following actions on the client side

The client will try to connect to the server with the specified information.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   22   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF4  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x03  | Packet header - sub packet type identifier |
| 4 | 16 | String |  | IpAddress |
| 20 | 2 | ShortLittleEndian |  | Port |