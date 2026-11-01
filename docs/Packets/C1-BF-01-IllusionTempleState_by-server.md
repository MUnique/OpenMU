# C1 BF 01 - IllusionTempleState (by server)

## Is sent when

The player is in the illusion temple event and the server sends a cyclic update.

## Causes the following actions on the client side

The client shows the score board, the remaining time, and the carrier of the holy relic and the own team mates on its mini map.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBF  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x01  | Packet header - sub packet type identifier |
| 4 | 2 | ShortLittleEndian |  | RemainingSeconds |
| 6 | 2 | ShortLittleEndian |  | RelicCarrierId |
| 8 | 1 | Byte |  | PositionX |
| 9 | 1 | Byte |  | PositionY |
| 10 | 1 | Byte |  | AlliedForcesPoints |
| 11 | 1 | Byte |  | IllusionForcesPoints |
| 12 | 1 | Byte |  | MyTeam |
| 13 | 1 | Byte |  | PartyCount |
| 14 | IllusionTempleTeamMate.Length *  | Array of IllusionTempleTeamMate |  | TeamMates |

### IllusionTempleTeamMate Structure

Contains the info about a team mate in the illusion temple, so that the client can show him on its mini map. Only PartyCount entries are sent - there are no unused/zeroed slots.

Length: 5 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortLittleEndian |  | PlayerId |
| 2 | 1 | Byte |  | MapNumber |
| 3 | 1 | Byte |  | PositionX |
| 4 | 1 | Byte |  | PositionY |