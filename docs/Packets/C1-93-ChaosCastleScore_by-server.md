# C1 93 - ChaosCastleScore (by server)

## Is sent when

The chaos castle mini game ended and the score of the player is sent to the player.

## Causes the following actions on the client side

The client shows the experience and the killed monsters and players.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   29   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x93  | Packet header - packet type identifier |
| 3 | 1 | Boolean |  | Success; If the player won the chaos castle. |
| 4 | 1 | Byte | 0xFE | Type; Identifies the chaos castle result. The client shares the code 0x93 with the MiniGameScoreTable and the BloodCastleScore and distinguishes them by this byte. |
| 5 | 10 | String |  | PlayerName |
| 17 | 4 | IntegerLittleEndian |  | MonsterKillCount |
| 21 | 4 | IntegerLittleEndian |  | BonusExperience |
| 25 | 4 | IntegerLittleEndian |  | PlayerKillCount |