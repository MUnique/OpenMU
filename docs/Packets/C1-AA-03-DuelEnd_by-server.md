# C1 AA 03 - DuelEnd (by server)

## Is sent when

After a duel ended.

## Causes the following actions on the client side

The client updates its state.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   17   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xAA  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x03  | Packet header - sub packet type identifier |
| 4 | 1 | Byte | 0 | Result |
| 5 | 2 | ShortBigEndian |  | OpponentId |
| 7 | 10 | String |  | OpponentName |