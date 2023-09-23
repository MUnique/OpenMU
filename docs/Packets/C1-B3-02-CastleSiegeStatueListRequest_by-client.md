# C1 B3 02 - CastleSiegeStatueListRequest (by client)

## Is sent when

The guild master opened the castle npc and the client needs a list of all statues.

## Causes the following actions on the server side

The server returns the list of statues and their status.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x02  | Packet header - sub packet type identifier |