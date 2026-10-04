# C1 BD 07 - CrywolfPersonalRank (by server)

## Is sent when

The crywolf event ended, after the CrywolfInfo with the state End.

## Causes the following actions on the client side

The client shows the rank and the experience of the player in the result.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   12   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBD  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x07  | Packet header - sub packet type identifier |
| 4 | 1 | CrywolfRank |  | Rank; The rank of the player. |
| 8 | 4 | IntegerLittleEndian |  | Experience; The rewarded experience. The field is aligned to 4 bytes, because the client structure is not packed. |

### CrywolfRank Enum

The rank of a player in the crywolf event.

| Value | Name | Description |
|-------|------|-------------|
| 0 | D | The rank D. |
| 1 | C | The rank C. |
| 2 | B | The rank B. |
| 3 | A | The rank A. |
| 4 | S | The rank S. |