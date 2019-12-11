# C1 F6 30 - Available Quests Request

## Is sent when
The client opened an quest NPC dialog and requests a list of available quests.


## Causes the following actions on the server side
The server sends the [list of available quests](<C1F60A - Quest List Response (by server).md>) of this NPC.


## Structure

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1    | [Packet type](PacketTypes.md) |
| 1 | byte | 0x04    | Packet header - length of the packet |
| 1 | byte | 0xF6    | Packet header - packet type identifier |
| 1 | byte | 0x30    | Packet header - packet type identifier |
