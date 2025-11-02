# C1 B7 01 - FireCatapultRequest (by client)

## Is sent when

A player wants to fire a catapult during the castle siege event.

## Causes the following actions on the server side

The server fires the catapult.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   7   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB7  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x01  | Packet header - sub packet type identifier |
| 4 | 2 | ShortBigEndian |  | CatapultId |
| 6 | 1 | Byte |  | TargetAreaIndex |