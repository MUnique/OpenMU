# C1 F6 31 - Request Buff

## Is sent when
The client opened an quest NPC dialog and requests a buff.
As far as I know, only the 'Elf Soldier' NPC offers such a buff until a certain level (150 or 220).


## Causes the following actions on the server side
The server should check if the correct Quest NPC (e.g. Elf Soldier) dialog is opened
and the player didn't reach the level limit yet.
If that's both the case, it adds a defined buff (MagicEffect) to the player; Otherwise, a message is sent to the player.


## Structure

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1    | [Packet type](PacketTypes.md) |
| 1 | byte | 0x04    | Packet header - length of the packet |
| 1 | byte | 0xF6    | Packet header - packet type identifier |
| 1 | byte | 0x31    | Packet header - packet type identifier |
