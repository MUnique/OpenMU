# C1 BF 0B - LuckyCoinCountRequest (by client)

## Is sent when

The player has the lucky coin dialog open and requests the current count of the registered coins.

## Causes the following actions on the server side

The server returns the count of the registered coins.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBF  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x0B  | Packet header - sub packet type identifier |