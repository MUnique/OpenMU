# C1 BF 09 - IllusionTempleEventState (by server)

## Is sent when

The state of an illusion temple event changed, e.g. when the battle starts.

## Causes the following actions on the client side

The client shows or hides the user interface of the event - the score board, the timer and the mini map - and applies the barriers of the arena, which are hardcoded at client side.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBF  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x09  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | TempleNumber |
| 5 | 1 | EventState |  | State |

### EventState Enum

Defines the state of an illusion temple event.

| Value | Name | Description |
|-------|------|-------------|
| 0 | WaitingRoom | The player entered the event and waits for it to start. It's only sent to the entering player, not to all participants. |
| 1 | Preparation | The preparation started: the players have been moved into the arena and assigned to their teams. The client opens the event interface with the score board, the timer and the mini map. |
| 2 | BattleStarted | The battle started: the statues are up and the barriers of the arena are removed, so that the players can reach the cursed statue. |
| 3 | Ended | The battle ended - the client closes the event interface. |