# C1 BF 0D - LuckyCoinExchangeResult (by server)

## Is sent when

After the player requested to exchange lucky coins.

## Causes the following actions on the client side

The client shows a message depending on the result.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBF  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x0D  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Result |