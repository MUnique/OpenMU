# C1 D1 00 - KanturuInfoRequest (by client)

## Is sent when

A player talks with the kanturu entrance npc, and shows the enter dialog.

## Causes the following actions on the server side

The server returns data about the state of the kanturu event map.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD1  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x00  | Packet header - sub packet type identifier |