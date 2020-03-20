# C2 F4 06 - ServerListResponse (by server)

## Is sent when

This packet is sent by the server after the client requested the current server list.

## Causes the following actions on the client side

The client shows the available servers with their load information.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0xF4  | Packet header - packet type identifier |
| 4 | 1 |    Byte   | 0x06  | Packet header - sub packet type identifier |
| 5 | 2 | ShortBigEndian |  | ServerCount |
| 7 | ServerLoadInfo.Length *  | Array of ServerLoadInfo |  | Servers |

### ServerLoadInfo Structure

Contains the id and the load of a server.

Length: 4 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortLittleEndian |  | ServerId |
| 2 | 1 | Byte |  | LoadPercentage |