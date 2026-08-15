# C1 B9 05 - CastleSiegeHuntingZoneEnterResponse (by server)

## Is sent when

A player requests to enter the Castle Siege hunting zone.

## Causes the following actions on the client side

The client closes the dialog on success or shows an error.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB9  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x05  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Result |