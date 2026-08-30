# C1 D1 03 - KanturuStateChange (by server)

## Is sent when

The Kanturu event transitions to a new phase or sub-phase.

## Causes the following actions on the client side

The client shows or hides the in-map HUD, switches background music, and when entering the Tower state reloads the barrier-open terrain file (EncTerrain_n_01.att) to visually remove the Elphis barrier.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD1  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x03  | Packet header - sub packet type identifier |
| 4 | 1 | StateType |  | State; Refers to the KanturuStateInfo.StateType enum values. |
| 5 | 1 | Byte |  | DetailState; Detail state within the main state. Maya battle: 0=none, 2=notify, 3=monster1, 4=maya1, 8=monster2, 9=maya2, 13=monster3, 14=maya3, 16=endcycle. Nightmare: 0=none, 1=idle, 2=intro, 3=battle, 4=end. Tower: 0=none, 1=revitalization, 2=notify, 3=close. |

### StateType Enum

Main state; see KanturuStateInfo.StateType for value descriptions.

| Value | Name | Description |
|-------|------|-------------|
| 0 | None |  |
| 1 | Standby |  |
| 2 | MayaBattle |  |
| 3 | NightmareBattle |  |
| 4 | Tower |  |
| 5 | End |  |