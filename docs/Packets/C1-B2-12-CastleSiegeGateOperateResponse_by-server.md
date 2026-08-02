# C1 B2 12 - CastleSiegeGateOperateResponse (by server)

## Is sent when

After a guild member of the castle owner requested to toggle a castle gate.

## Causes the following actions on the client side

The client shows the result of the gate toggle operation.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   7   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x12  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Result |
| 5 | 2 | ShortBigEndian |  | GateIndex |