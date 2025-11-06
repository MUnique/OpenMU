# C1 0F - WeatherStatusUpdate (by server)

## Is sent when

The weather on the current map has been changed or the player entered the map.

## Causes the following actions on the client side

The game client updates the weather effects.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x0F  | Packet header - packet type identifier |
| 3 | 4 bit | Byte |  | Weather; A random value between 0 and 2 (inclusive). |
| 3 | 4 bit | Byte |  | Variation; A random value between 0 and 9 (inclusive). |