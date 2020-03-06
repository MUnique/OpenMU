# C1 15 - InstantMoveRequest (by client)

## Is sent when

It's sent when the player performs specific skills.

## Causes the following actions on the server side

Usually, the player is moved instantly to the specified coordinates on the current map. In OpenMU, this request is not handled, because it allows hackers to "teleport" to any coordinates.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x15  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | TargetX |
| 4 | 1 | Byte |  | TargetY |