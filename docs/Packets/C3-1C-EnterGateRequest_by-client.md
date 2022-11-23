# C3 1C - EnterGateRequest (by client)

## Is sent when

Usually: When the player enters an area on the game map which is configured as gate at the client data files. In the special case of wizards, this packet is also used for the teleport skill. When this is the case, GateNumber is 0 and the target coordinates are specified.

## Causes the following actions on the server side

If the player is allowed to enter the "gate", it's moved to the corresponding exit gate area.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   8   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x1C  | Packet header - packet type identifier |
| 4 | 2 | ShortLittleEndian |  | GateNumber |
| 6 | 1 | Byte |  | TeleportTargetX |
| 7 | 1 | Byte |  | TeleportTargetY |