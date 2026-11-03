# C1 B8 01 - KillCountUpdate (by server)

## Is sent when

After the kill count of the character changed.

## Causes the following actions on the client side

The client stores the kill count (0 resets it).

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB8  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x01  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | KillCount |