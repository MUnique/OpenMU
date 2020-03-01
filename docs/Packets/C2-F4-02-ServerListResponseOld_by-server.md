# C2 F4 02 - ServerListResponseOld (by server)

## Is sent when

This packet is sent by the server (below season 1) after the client requested the current server list.

## Causes the following actions on the client side

The client shows the available servers with their load information.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0xF4  | Packet header - packet type identifier |
| 4 | 1 |    Byte   | 0x02  | Packet header - sub packet type identifier |
| 5 | 1 | Byte |  | ServerCount |
| 6 | ServerLoadInfo.Length *  | Array of ServerLoadInfo |  | Servers |

### ServerLoadInfo Structure

Contains the id and the load of a server.

Length: 2 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte |  | ServerId |
| 1 | 1 | Byte |  | LoadPercentage |