# C1 AA - DuelStartResultSeason3 (by server)

## Is sent when

After the client sent a DuelStartRequestSeason3, and it either failed or the requested player sent a response. It's the packet for the clients before Season 4, which don't know the result codes of the DuelStartResult and get the reason of a failure as a message instead.

## Causes the following actions on the client side

The client shows the started duel, or that it has not been started.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   16   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xAA  | Packet header - packet type identifier |
| 3 | 1 | DuelStartResultSeason3Type |  | Result |
| 4 | 2 | ShortBigEndian |  | OpponentId |
| 6 | 10 | String |  | OpponentName |

### DuelStartResultSeason3Type Enum

Describes the result of a duel request at the clients before Season 4, which only know whether the duel started. The reason why it didn't start is sent as a message instead.

| Value | Name | Description |
|-------|------|-------------|
| 0 | NotStarted | The duel has not been started, e.g. because the opponent refused it. |
| 1 | Started | The duel has been started. |