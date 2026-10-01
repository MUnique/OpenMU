# C1 F8 07 - GensInfo (by server)

## Is sent when

After the player entered the game world, joined or left a gens, or requested the gens ranking.

## Causes the following actions on the client side

The client shows the gens mark and rank of the own character, and the gens info window shows the ranking and contribution.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   24   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF8  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x07  | Packet header - sub packet type identifier |
| 4 | 1 | GensType |  | GensType |
| 8 | 4 | IntegerLittleEndian |  | RankingPosition |
| 12 | 4 | IntegerLittleEndian |  | Rank |
| 16 | 4 | IntegerLittleEndian |  | ContributionPoints |
| 20 | 4 | IntegerLittleEndian |  | NextRankContributionPoints |

### GensType Enum

Describes the gens type.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Undefined | The undefined gens type, e.g. when the player is not a member of a gens. |
| 1 | Duprian | The Duprian gens. |
| 2 | Vanert | The Vanert gens. |