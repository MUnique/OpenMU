# C1 BF 51 - MuHelperStatusUpdate (by server)

## Is sent when

The server validated or changed the status of the MU Helper.

## Causes the following actions on the client side

The client toggle the MU Helper status.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   16   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBF  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x51  | Packet header - sub packet type identifier |
| 4 | 1 | Boolean |  | ConsumeMoney |
| 8 | 4 | IntegerLittleEndian |  | Money |
| 12 | 1 | Boolean |  | PauseStatus |