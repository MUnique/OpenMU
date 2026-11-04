# C1 BF 04 - IllusionTempleResult (by server)

## Is sent when

The illusion temple event ended.

## Causes the following actions on the client side

The client shows the results.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBF  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x04  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Team1Points |
| 5 | 1 | Byte |  | Team2Points |
| 6 | 1 | Byte |  | PlayerCount |
| 10 | PlayerResult.Length * PlayerCount | Array of PlayerResult |  | Players |

### PlayerResult Structure

Contains the result of a player in the event.

Length: 17 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 |  | String |  | Name |
| 10 | 1 | Byte |  | MapNumber |
| 11 | 1 | Byte |  | Team |
| 12 | 1 | Byte |  | Class |
| 13 | 4 | IntegerLittleEndian |  | AddedExperience |