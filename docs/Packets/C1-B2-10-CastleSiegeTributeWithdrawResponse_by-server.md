# C1 B2 10 - CastleSiegeTributeWithdrawResponse (by server)

## Is sent when

After the guild master requested to withdraw tax money from the castle treasury.

## Causes the following actions on the client side

The client shows the result of the withdrawal and the withdrawn amount.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   13   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x10  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Result |
| 5 | 8 | LongBigEndian |  | Money |