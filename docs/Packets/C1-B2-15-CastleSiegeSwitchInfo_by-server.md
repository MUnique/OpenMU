# C1 B2 15 - CastleSiegeSwitchInfo (by server)

## Is sent when

The server sends information about a castle siege switch (e.g., life stone activation).

## Causes the following actions on the client side

The client updates the switch state display.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   29   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x15  | Packet header - sub packet type identifier |
| 4 | 2 | ShortBigEndian |  | SwitchIndex |
| 6 | 1 | Byte |  | SwitchId |
| 7 | 1 | Byte |  | State |
| 8 | 1 | Byte |  | JoinSide |
| 9 | 9 | String |  | GuildName |
| 18 | 11 | String |  | UserName |