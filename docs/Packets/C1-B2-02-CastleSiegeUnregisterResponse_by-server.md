# C1 B2 02 - CastleSiegeUnregisterResponse (by server)

## Is sent when

After the player requested to un-register his guild from the next castle siege.

## Causes the following actions on the client side

The client shows the result of the un-registration attempt.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x02  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Result |