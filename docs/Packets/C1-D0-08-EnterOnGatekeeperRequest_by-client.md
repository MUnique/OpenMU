# C1 D0 08 - EnterOnGatekeeperRequest (by client)

## Is sent when

A player is running the quest "Into the 'Darkness' Zone" (nr. 6), talking to the gatekeeper npc in 'Barracks of Balgass'.

## Causes the following actions on the server side

It will warp the player to the map 'Balgass Refuge' where the required monsters have to be killed to proceed with the quest.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD0  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x08  | Packet header - sub packet type identifier |