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
| 6 | 1 | Byte |  | State |
| 7 | 1 | Byte |  | JoinSide |
| 8 | 8 | String |  | GuildName |
| 16 | 11 | String |  | UserName |