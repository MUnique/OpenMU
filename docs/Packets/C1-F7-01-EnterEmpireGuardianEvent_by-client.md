# C1 F7 01 - EnterEmpireGuardianEvent (by client)

## Is sent when

The player wants to enter the empire guardian event due an npc dialog.

## Causes the following actions on the server side

The checks if the player can enter the event, and moves it to the event, if possible.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF7  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x01  | Packet header - sub packet type identifier |
| 4 | 1 | Byte | 01 | ItemSlot; The item slot of the event ticket. Not used by the server. |