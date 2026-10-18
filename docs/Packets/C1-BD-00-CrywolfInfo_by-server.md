# C1 BD 00 - CrywolfInfo (by server)

## Is sent when

The state of the crywolf event changed, or the player entered the crywolf map. It should only be sent to players on the crywolf map, because the client loads the terrain of the occupation state for its current map.

## Causes the following actions on the client side

The client shows the event window in the states Ready, Start and End, plays the intro in the state Notify2, shows the result in the state End, and loads the terrain and light of the occupation state.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBD  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x00  | Packet header - sub packet type identifier |
| 4 | 1 | OccupationState |  | Occupation; The occupation state. It must be Peace or Occupied in the state End, otherwise the client shows no result. |
| 5 | 1 | CrywolfState |  | State; The state of the event. |

### OccupationState Enum

The occupation state of the crywolf fortress.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Peace | The fortress has been defended. The benefits apply. |
| 1 | Occupied | The fortress has been occupied by Balgass. The penalties apply. |
| 2 | War | The fortress is under attack. |

### CrywolfState Enum

The state of the crywolf event.

| Value | Name | Description |
|-------|------|-------------|
| 0 | None | The event is not running. |
| 1 | Notify1 | The first notification of the upcoming event. |
| 2 | Notify2 | The second notification. The client plays the intro of the event. |
| 3 | Ready | The altars can be contracted and the monsters appeared. |
| 4 | Start | The monsters attack. |
| 5 | End | The event ended. The client shows the result. |
| 6 | EndCycle | The event is finished. |