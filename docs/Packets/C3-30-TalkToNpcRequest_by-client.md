# C3 30 - TalkToNpcRequest (by client)

## Is sent when

A player wants to talk to an NPC.

## Causes the following actions on the server side

Based on the NPC type, the server sends a response back to the game client. For example, if it's a merchant NPC, it sends back that a merchant dialog should be opened and which items are offered by this NPC.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x30  | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | NpcId |