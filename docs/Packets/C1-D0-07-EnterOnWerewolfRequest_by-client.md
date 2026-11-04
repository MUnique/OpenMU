# C1 D0 07 - EnterOnWerewolfRequest (by client)

## Is sent when

A player is running the quest "Infiltrate The Barracks of Balgass" (nr. 5), talking to the Werewolf npc in Crywolf.

## Causes the following actions on the server side

It will warp the player to the map 'Barracks of Balgass' where the required monsters have to be killed to proceed with the quest.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD0  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x07  | Packet header - sub packet type identifier |