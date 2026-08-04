# C1 B2 20 - CastleSiegeSwitchInfo (by server)

## Is sent when

The server sends information about a Castle Siege crown switch.

## Causes the following actions on the client side

The client updates the crown switch occupation display.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   27   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x20  | Packet header - sub packet type identifier |
| 4 | 2 | ShortBigEndian |  | SwitchIndex |
| 6 | 1 | Boolean |  | IsOccupied |
| 7 | 1 | CastleSiegeJoinSide |  | JoinSide |
| 8 | 8 | String |  | GuildName |
| 16 | 11 | String |  | UserName |

### CastleSiegeJoinSide Enum

Defines a participant's side in the Castle Siege battle.

| Value | Name | Description |
|-------|------|-------------|
| 0 | None | The participant does not belong to a siege side. |
| 1 | Defense | The defending alliance. |
| 2 | Attack1 | The first attacking alliance. |
| 3 | Attack2 | The second attacking alliance. |
| 4 | Attack3 | The third attacking alliance. |