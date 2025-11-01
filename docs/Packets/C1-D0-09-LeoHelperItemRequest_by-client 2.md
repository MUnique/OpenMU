# C1 D0 09 - LeoHelperItemRequest (by client)

## Is sent when

The player talks to the npc "Leo the Helper" and requests an item.

## Causes the following actions on the server side

The item will drop on the ground.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD0  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x09  | Packet header - sub packet type identifier |