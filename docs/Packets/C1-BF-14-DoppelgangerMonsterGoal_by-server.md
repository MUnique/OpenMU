# C1 BF 14 - DoppelgangerMonsterGoal (by server)

## Is sent when

A monster reached the magic circle during the doppelganger event.

## Causes the following actions on the client side

The client updates the counter of monsters which passed the magic circle.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBF  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x14  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | MaximumGoalCount; The number of monsters which may reach the magic circle until the event fails. |
| 5 | 1 | Byte |  | GoalCount; The number of monsters which reached the magic circle. |