# C1 1C - EnterGateRequest (by client)

## Is sent when

When the player enters an area on the game map which is configured as gate at the client data files.

## Causes the following actions on the server side

If the player is allowed to enter the "gate", it's moved to the corresponding exit gate area.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x1C  | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | GateNumber |