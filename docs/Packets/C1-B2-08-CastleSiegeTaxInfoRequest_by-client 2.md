# C1 B2 08 - CastleSiegeTaxInfoRequest (by client)

## Is sent when

The guild master opened a castle siege npc to manage the castle.

## Causes the following actions on the server side

The server returns the tax information.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x08  | Packet header - sub packet type identifier |