# C1 D1 07 - KanturuMonsterUserCount (by server)

## Is sent when

A monster is killed or the player count changes during the Kanturu event.

## Causes the following actions on the client side

The client updates the monster count and user count numbers displayed in the Kanturu HUD.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD1  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x07  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | MonsterCount; Number of monsters still alive in the current wave (capped at 255). |
| 5 | 1 | Byte |  | UserCount; Number of players currently inside the event map (capped at 255). |