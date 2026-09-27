# C1 D1 13 - RaklionBattleResult (by server)

## Is sent when

The battle against Selupan ended.

## Causes the following actions on the client side

None, the client ignores it.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD1  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x13  | Packet header - sub packet type identifier |
| 4 | 1 | BattleResult |  | Result |

### BattleResult Enum

The result of the battle against Selupan.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Failure | All players of the battle died or left. |
| 1 | Success | Selupan was killed. |