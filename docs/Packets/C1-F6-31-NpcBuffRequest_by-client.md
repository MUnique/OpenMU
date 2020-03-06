# C1 F6 31 - NpcBuffRequest (by client)

## Is sent when

The game client requests to get a buff from the currently interacting quest npc. As far as we know, only the Elf Soldier NPC offers such a buff until a certain character level (150 or 220).

## Causes the following actions on the server side

The server should check if the correct Quest NPC (e.g. Elf Soldier) dialog is opened and the player didn't reach the level limit yet. If that's both the case, it adds a defined buff (MagicEffect) to the player; Otherwise, a message is sent to the player.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF6  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x31  | Packet header - sub packet type identifier |