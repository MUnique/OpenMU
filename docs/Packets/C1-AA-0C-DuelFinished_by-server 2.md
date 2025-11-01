# C1 AA 0C - DuelFinished (by server)

## Is sent when

When the duel finished.

## Causes the following actions on the client side

The client shows the winner and loser names.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   24   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xAA  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x0C  | Packet header - sub packet type identifier |
| 4 | 10 | String |  | Winner |
| 14 | 10 | String |  | Loser |