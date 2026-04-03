# C1 B7 05 - CastleSiegeLifeStoneState (by server)

## Is sent when

The server sends the current state of the castle life stone.

## Causes the following actions on the client side

The client updates the life stone state display.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB7  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x05  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | State; The creation stage of the life stone (0 to 4). |