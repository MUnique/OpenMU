# C1 B2 01 - CastleSiegeRegistrationResponse (by server)

## Is sent when

After the player requested to register his guild for the next castle siege.

## Causes the following actions on the client side

The client shows the result of the registration attempt.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   13   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x01  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Result |
| 5 | 8 | String |  | GuildName |