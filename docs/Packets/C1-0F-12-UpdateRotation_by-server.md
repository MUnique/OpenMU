# C1 0F 12 - UpdateRotation (by server)

## Is sent when

The player's rotation has been updated after entering a map or teleporting.

## Causes the following actions on the client side

The game client updates the player's rotation/direction.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x0F  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x12  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Rotation; The rotation direction of the character. |