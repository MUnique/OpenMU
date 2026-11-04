# C1 AA 06 - DuelStatus (by server)

## Is sent when

When the client requested the list of the current duel rooms.

## Causes the following actions on the client side

The client shows the list of duel rooms.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   92   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xAA  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x06  | Packet header - sub packet type identifier |
| 4 | DuelRoomStatus.Length *  | Array of DuelRoomStatus |  | Rooms |

### DuelRoomStatus Structure

Structure for a duel room entry.

Length: 22 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 10 | String |  | Player1Name |
| 10 | 10 | String |  | Player2Name |
| 20 | 1 | Boolean |  | DuelRunning |
| 21 | 1 | Boolean |  | DuelOpen |