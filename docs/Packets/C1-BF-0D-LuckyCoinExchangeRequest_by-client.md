# C1 BF 0D - LuckyCoinExchangeRequest (by client)

## Is sent when

The player has the lucky coin dialog open and requests an exchange for the specified number of registered coins.

## Causes the following actions on the server side

The server adds an item to the inventory of the character and sends a response with a result code.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   8   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBF  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x0D  | Packet header - sub packet type identifier |
| 4 | 4 | IntegerLittleEndian |  | CoinCount |