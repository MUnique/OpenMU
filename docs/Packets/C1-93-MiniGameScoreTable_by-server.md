# C1 93 - MiniGameScoreTable (by server)

## Is sent when

A mini game ended and the score table is sent to the player.

## Causes the following actions on the client side

The score table is shown at the client.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x93  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | PlayerRank |
| 4 | 1 | Byte |  | ResultCount; The number of results. The client shares the code 0x93 with the BloodCastleScore (0xFF) and ChaosCastleScore (0xFE) at this index, so a score table must contain less than 200 results; the client holds up to 11. |
| 5 | ResultItem.Length * ResultCount | Array of ResultItem |  | Results |

### ResultItem Structure

The result of one player.

Length: 24 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 10 | String |  | PlayerName |
| 12 | 4 | IntegerLittleEndian |  | TotalScore |
| 16 | 4 | IntegerLittleEndian |  | BonusExperience |
| 20 | 4 | IntegerLittleEndian |  | BonusMoney |