# C1 BF 0B - LuckyCoinCount (by server)

## Is sent when

After the player requested the count of registered lucky coins.

## Causes the following actions on the client side

The client shows the count.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   8   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBF  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x0B  | Packet header - sub packet type identifier |
| 4 | 4 | IntegerLittleEndian |  | Count |