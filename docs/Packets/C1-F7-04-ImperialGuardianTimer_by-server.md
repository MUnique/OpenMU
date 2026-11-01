# C1 F7 04 - ImperialGuardianTimer (by server)

## Is sent when

Every second during the imperial guardian event.

## Causes the following actions on the client side

The client shows the timer with the remaining time and the number of remaining monsters. The client doesn't count down the time by itself.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   16   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF7  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x04  | Packet header - sub packet type identifier |
| 4 | 1 | TimerType |  | Type; The type of the timer. |
| 8 | 4 | IntegerLittleEndian |  | RemainingMilliseconds; The remaining time in milliseconds. The field is aligned to 4 bytes, because the client structure is not packed. |
| 12 | 1 | Byte |  | MonsterCount; The number of remaining monsters. |

### TimerType Enum

The type of the timer.

| Value | Name | Description |
|-------|------|-------------|
| 0 | LootTime | The time to collect the loot. |
| 1 | Standby | The time until the monsters appear. |
| 2 | TimeAttack | The time to kill the monsters. |