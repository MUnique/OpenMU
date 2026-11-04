# C1 D1 00 - KanturuStateInfo (by server)

## Is sent when

The player requests state information from the Kanturu gateway NPC.

## Causes the following actions on the client side

The client shows the Kanturu entry dialog (INTERFACE_KANTURU2ND_ENTERNPC) with event state, detail state, whether entry is possible, current player count and remaining time.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   12   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD1  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x00  | Packet header - sub packet type identifier |
| 4 | 1 | StateType |  | State |
| 5 | 1 | Byte |  | DetailState; Detail state; semantics depend on the main State field. See the game logic enums for per-state values. |
| 6 | 1 | Boolean |  | CanEnter; 1 = entrance is open (Enter button enabled); 0 = entrance closed. |
| 7 | 1 | Byte |  | UserCount; Number of players currently inside the event map (capped at 255). |
| 8 | 4 | IntegerLittleEndian |  | RemainSeconds; Remaining time in seconds. Standby: seconds until event opens. Tower: seconds the tower has been open. Otherwise 0. |

### StateType Enum

Main state of the Kanturu event, matching the client KANTURU_STATE_TYPE enum.

| Value | Name | Description |
|-------|------|-------------|
| 0 | None | No active state. |
| 1 | Standby | Waiting for players to enter before the event starts. |
| 2 | MayaBattle | Maya battle phase covering Phases 1 through 3 and their boss waves. |
| 3 | NightmareBattle | Nightmare battle phase after all three Maya phases are cleared. |
| 4 | Tower | Tower of Refinement phase; opens after Nightmare is defeated. |
| 5 | End | Event has ended. |