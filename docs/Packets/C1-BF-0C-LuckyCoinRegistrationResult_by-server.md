# C1 BF 0C - LuckyCoinRegistrationResult (by server)

## Is sent when

After the player registered a lucky coin.

## Causes the following actions on the client side

The client shows the result and the new count.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   12   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBF  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x0C  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Result |
| 8 | 4 | IntegerLittleEndian |  | Count |