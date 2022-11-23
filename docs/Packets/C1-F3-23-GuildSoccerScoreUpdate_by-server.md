# C1 F3 23 - GuildSoccerScoreUpdate (by server)

## Is sent when

Whenever the score of the soccer game changed, and at the beginning of the match.

## Causes the following actions on the client side

The score is updated on the user interface.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   22   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x23  | Packet header - sub packet type identifier |
| 4 | 8 | String |  | RedTeamName |
| 12 | 1 | Byte |  | RedTeamGoals |
| 13 | 8 | String |  | BlueTeamName |
| 21 | 1 | Byte |  | BlueTeamGoals |