# C3 B0 - TeleportTarget (by client)

## Is sent when

A wizard uses the 'Teleport Ally' skill to teleport a party member of his view range to a nearby coordinate.

## Causes the following actions on the server side

If the target player is in the same party and in the range, it will teleported to the specified coordinates.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   7   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB0  | Packet header - packet type identifier |
| 3 | 2 | ShortLittleEndian |  | TargetId |
| 5 | 1 | Byte |  | TeleportTargetX |
| 6 | 1 | Byte |  | TeleportTargetY |