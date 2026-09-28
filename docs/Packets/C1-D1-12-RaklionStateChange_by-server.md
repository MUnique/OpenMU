# C1 D1 12 - RaklionStateChange (by server)

## Is sent when

The state of the raklion event or of Selupan changed.

## Causes the following actions on the client side

The client updates the state of the raklion maps, e.g. whether the portal to the hatchery is shown, the effects and the music.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD1  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x12  | Packet header - sub packet type identifier |
| 4 | 1 | RaklionState |  | State |
| 5 | 1 | Byte |  | DetailState; The state of Selupan, if the state is DetailState: 0 = none, 1 = standby, 2 to 8 = pattern 1 to 7 (by the remaining health), 9 = dead. Otherwise it is ignored by the client. |

### RaklionState Enum

The state of the raklion event.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Idle | No event activity; the hatchery gate is open. |
| 1 | Notify1 | Only a few spider eggs are left. |
| 2 | Standby | All spider eggs are destroyed and Selupan is about to appear. |
| 3 | Notify2 | Selupan appeared; the hatchery gate closes soon. |
| 4 | Ready | Selupan rises; the client plays the boss music on the hatchery map. |
| 5 | StartBattle | The battle against Selupan is running. |
| 6 | Notify3 | The hatchery gate is about to close. |
| 7 | CloseDoor | The hatchery gate is closed; nobody can enter anymore. |
| 8 | AllUserDie | All players of the battle died or left. |
| 9 | Notify4 | The battle ended; the hatchery gate opens soon. |
| 10 | End | The event ended. |
| 11 | DetailState | The detail state contains the state of Selupan. |