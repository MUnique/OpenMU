# C1 BF 01 - IllusionTempleState (by server)

## Is sent when

The player is in the illusion temple event and the server sends a cyclic update.

## Causes the following actions on the client side

The client shows the state in the user interface.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBF  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x01  | Packet header - sub packet type identifier |
| 4 | 2 | ShortLittleEndian |  | RemainingSeconds |
| 4 | 2 | ShortLittleEndian |  | PlayerIndex |
| 6 | 1 | Byte |  | PositionX |
| 7 | 1 | Byte |  | PositionY |
| 8 | 1 | Byte |  | Team1Points |
| 9 | 1 | Byte |  | Team2Points |
| 10 | 1 | Byte |  | MyTeam |
| 11 | 1 | Byte |  | PartyCount |
| 12 | IllusionTemplePartyEntry.Length *  | Array of IllusionTemplePartyEntry |  | PartyMembers |

### IllusionTemplePartyEntry Structure

Contains the info about a party member in illusion temple.

Length: 5 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortLittleEndian |  | PlayerId |
| 2 | 2 | ShortLittleEndian |  | MapNumber |
| 3 | 1 | Byte |  | PositionX |
| 4 | 1 | Byte |  | PositionY |