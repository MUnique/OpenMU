# C1 F4 03 - ConnectionInfoRequest (by client)

## Is sent when

This packet is sent by the client after the user clicked on an entry of the server list.

## Causes the following actions on the server side

The server will send a ConnectionInfo back to the client.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF4  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x03  | Packet header - sub packet type identifier |
| 4 | 2 | ShortLittleEndian |  | ServerId |