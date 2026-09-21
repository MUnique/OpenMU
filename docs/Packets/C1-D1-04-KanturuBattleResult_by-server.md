# C1 D1 04 - KanturuBattleResult (by server)

## Is sent when

The Kanturu event ends with a victory or defeat outcome.

## Causes the following actions on the client side

The client displays the Success_kantru.tga overlay on victory or the Failure_kantru.tga overlay on defeat.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD1  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x04  | Packet header - sub packet type identifier |
| 4 | 1 | BattleResult |  | Result |

### BattleResult Enum

Outcome of the Kanturu battle.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Failure | The event ended in failure; shows Failure_kantru.tga. |
| 1 | Victory | Nightmare was defeated; shows Success_kantru.tga. |