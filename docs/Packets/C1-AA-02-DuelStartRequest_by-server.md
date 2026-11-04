# C1 AA 02 - DuelStartRequest (by server)

## Is sent when

After another client sent a DuelStartRequest, to ask the requested player for a response.

## Causes the following actions on the client side

The client shows the duel request.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   16   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xAA  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x02  | Packet header - sub packet type identifier |
| 4 | 2 | ShortLittleEndian |  | RequesterId |
| 6 | 10 | String |  | RequesterName |