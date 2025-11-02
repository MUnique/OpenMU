# C1 D0 0A - MoveToDeviasBySnowmanRequest (by client)

## Is sent when

The player talks to the npc "Snowman" in Santa Village and requests to warp back to devias.

## Causes the following actions on the server side

The player will be warped back to Devias.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD0  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x0A  | Packet header - sub packet type identifier |