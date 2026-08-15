# C1 B2 03 - CastleSiegeRegistrationStateResponse (by server)

## Is sent when

After the player requested the current registration state of his guild.

## Causes the following actions on the client side

The client shows the current registration state including the number of submitted guild marks.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   19   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x03  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Result |
| 5 | 8 | String |  | GuildName |
| 13 | 4 | IntegerBigEndian |  | GuildMarkCount |
| 17 | 1 | Boolean |  | IsGivingUp |
| 18 | 1 | Byte |  | RegistrationRank |