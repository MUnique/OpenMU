# C1 8E 01 - MapMoveChecksumKey (by server)

## Is sent when

After entering the game, the server sends the key for the map move (warp) command.

## Causes the following actions on the client side

The client stores the key and uses it for the WarpCommandRequest.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   8   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x8E  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x01  | Packet header - sub packet type identifier |
| 4 | 4 | IntegerLittleEndian |  | Key |