# C1 93 - BloodCastleScore (by server)

## Is sent when

The blood castle mini game ended and the score of the player is sent to the player.

## Causes the following actions on the client side

The score is shown at the client.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   29   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x93  | Packet header - packet type identifier |
| 3 | 1 | Boolean |  | Success |
| 4 | 1 | Byte | 0xFF | Type |
| 5 | 10 | String |  | PlayerName |
| 19 | 4 | IntegerBigEndian |  | TotalScore |
| 21 | 4 | IntegerBigEndian |  | BonusExperience |
| 25 | 4 | IntegerBigEndian |  | BonusMoney |