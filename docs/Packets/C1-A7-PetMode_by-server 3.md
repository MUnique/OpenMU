# C1 A7 - PetMode (by server)

## Is sent when

After the client sent a PetAttackCommand (as confirmation), or when the previous command finished and the pet is reset to Normal-mode.

## Causes the following actions on the client side

The client updates the pet mode in its user interface.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   7   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xA7  | Packet header - packet type identifier |
| 3 | 1 | ClientToServer.PetType | ClientToServer.PetType.DarkRaven | Pet |
| 4 | 1 | ClientToServer.PetCommandMode |  | PetCommandMode |
| 5 | 2 | ShortBigEndian |  | TargetId |