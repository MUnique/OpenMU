# C2 F8 05 - AssignCharactersToGens (by server)

## Is sent when

The server wants to visibly assign players to their gens, e.g. when two players met each other, or when a player joined or left a gens.

## Causes the following actions on the client side

The client shows the gens mark and rank next to the names of the players.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0xF8  | Packet header - packet type identifier |
| 4 | 1 |    Byte   | 0x05  | Packet header - sub packet type identifier |
| 5 | 1 | Byte |  | PlayerCount |
| 6 | GensMemberRelation.Length * PlayerCount | Array of GensMemberRelation |  | Members |

### GensMemberRelation Structure

Relation between a gens and a player.

Length: 16 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | GensType |  | GensType |
| 1 | 2 | ShortBigEndian |  | PlayerId |
| 4 | 4 | IntegerLittleEndian |  | RankingPosition |
| 8 | 4 | IntegerLittleEndian |  | Rank |
| 12 | 4 | IntegerLittleEndian |  | ContributionPoints |

### GensType Enum

Describes the gens type.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Undefined | The undefined gens type, e.g. when the player is not a member of a gens. |
| 1 | Duprian | The Duprian gens. |
| 2 | Vanert | The Vanert gens. |