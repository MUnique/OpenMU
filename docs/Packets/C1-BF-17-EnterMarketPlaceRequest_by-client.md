# C1 BF 17 - EnterMarketPlaceRequest (by client)

## Is sent when

The player wants to enter the market place map.

## Causes the following actions on the server side

The server moves the player to the market place map.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBF  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x17  | Packet header - sub packet type identifier |