# C1 64 - GuildWarScoreUpdate (by server)

## Is sent when

The guild war score changed.

## Causes the following actions on the client side

The guild score is updated on the client side.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x64  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | ScoreOfOwnGuild |
| 4 | 1 | Byte |  | ScoreOfEnemyGuild |
| 5 | 1 | Byte | 0 | Type |