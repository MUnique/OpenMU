# C1 B2 01 - CastleSiegeRegistrationRequest (by client)

## Is sent when

The player opened a castle siege npc to register his guild alliance.

## Causes the following actions on the server side

The server returns the result of the castle siege registration.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x01  | Packet header - sub packet type identifier |