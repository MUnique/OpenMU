# C1 BD 09 - CrywolfChaosRateBenefitRequest (by client)

## Is sent when

A player opens an item crafting dialog, e.g. the chaos machine.

## Causes the following actions on the server side

The server returns data about the state of the benefit of the crywolf event. If it was won before, the chaos rate wents up a few percent.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBD  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x09  | Packet header - sub packet type identifier |