# C1 B2 04 - CastleSiegeMarkRegistrationResponse (by server)

## Is sent when

After the player submitted a guild mark for the castle siege registration.

## Causes the following actions on the client side

The client shows the updated guild mark count.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   17   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x04  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Result |
| 5 | 8 | String |  | GuildName |
| 13 | 4 | IntegerBigEndian |  | GuildMarkCount |