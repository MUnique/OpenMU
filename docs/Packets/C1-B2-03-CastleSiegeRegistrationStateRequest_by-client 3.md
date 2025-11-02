# C1 B2 03 - CastleSiegeRegistrationStateRequest (by client)

## Is sent when

The player opened a castle siege npc and requests the state about the own registration.

## Causes the following actions on the server side

The server returns the state of the castle siege registration, which includes the number of submitted guild marks.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x03  | Packet header - sub packet type identifier |