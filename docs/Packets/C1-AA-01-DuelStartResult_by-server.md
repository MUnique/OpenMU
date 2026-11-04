# C1 AA 01 - DuelStartResult (by server)

## Is sent when

After the client sent a DuelStartRequest, and it either failed or the requested player sent a response.

## Causes the following actions on the client side

The client shows the started or aborted duel.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   17   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xAA  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x01  | Packet header - sub packet type identifier |
| 4 | 1 | DuelStartResultType |  | Result |
| 5 | 2 | ShortBigEndian |  | OpponentId |
| 7 | 10 | String |  | OpponentName |

### DuelStartResultType Enum

Describes the type of the duel result.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Success | The duel has been started. |
| 12 | FailedByTooLowLevel | The duel couldn't be started, because one of the players has not the minimum level, usually 30. |
| 14 | FailedByError | The duel couldn't be started, because of an unexpected error. |
| 15 | Refused | The duel couldn't be started, because the opponent refused. |
| 16 | FailedByNoFreeRoom | The duel couldn't be started, because no duel room is free. |
| 28 | FailedBy_ | The duel couldn't be started, because ... |
| 30 | FailedByNotEnoughMoney | The duel couldn't be started, because one of the players has not enough money, usually 30000.  |