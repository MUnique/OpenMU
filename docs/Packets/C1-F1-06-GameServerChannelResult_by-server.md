# C1 F1 06 - GameServerChannelResult (by server)

## Is sent when

Answer to the client packet C1 F1 06 which is sent from the game server channel message box. Purpose not fully identified.

## Causes the following actions on the client side

If Result is 1, the client continues with the given value.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF1  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x06  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Result |
| 5 | 1 | Byte |  | Value |