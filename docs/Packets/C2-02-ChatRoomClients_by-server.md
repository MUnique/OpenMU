# C2 02 - ChatRoomClients (by server)

## Is sent when

This packet is sent by the server after another chat client sent a message to the current chat room.

## Causes the following actions on the client side

The client will show the message.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0x02  | Packet header - packet type identifier |
| 6 | 1 | Byte |  | ClientCount |
| 8 | ChatClient.Length * ClientCount | Array of ChatClient |  | Clients |

### ChatClient Structure

Contains the index and the name of a connected chat client in the room.

Length: 11 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte |  | Index |
| 1 | 10 | String |  | Name |