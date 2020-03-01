# C1 54 - GuildMasterAnswer (by client)

## Is sent when

The player has the dialog of the guild master NPC opened and decided about its next step.

## Causes the following actions on the server side

It either cancels the guild creation or proceeds with the guild creation dialog where the player can enter the guild name and symbol.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x54  | Packet header - packet type identifier |
| 3 | 1 | Boolean |  | ShowCreationDialog; A value whether the guild creation dialog should be shown. Otherwise, the guild creation is cancelled and the dialog was closed. |