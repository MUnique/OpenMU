# C1 F3 12 - ClientReadyAfterMapChange (by client)

## Is sent when

After the server sent a map change message and the client has initialized the game map visualization.

## Causes the following actions on the server side

The character is added to the internal game map and ready to interact with other entities.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x12  | Packet header - sub packet type identifier |