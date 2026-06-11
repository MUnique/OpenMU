# C1 B2 00 - CastleSiegeStatusResponse (by server)

## Is sent when

After the player requested the current castle siege status from a castle siege npc.

## Causes the following actions on the client side

The client shows the castle siege status.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   46   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x00  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Result |
| 5 | 1 | Byte |  | State |
| 6 | 2 | ShortBigEndian |  | StartYear |
| 8 | 1 | Byte |  | StartMonth |
| 9 | 1 | Byte |  | StartDay |
| 10 | 1 | Byte |  | StartHour |
| 11 | 1 | Byte |  | StartMinute |
| 12 | 2 | ShortBigEndian |  | EndYear |
| 14 | 1 | Byte |  | EndMonth |
| 15 | 1 | Byte |  | EndDay |
| 16 | 1 | Byte |  | EndHour |
| 17 | 1 | Byte |  | EndMinute |
| 18 | 2 | ShortBigEndian |  | SiegeStartYear |
| 20 | 1 | Byte |  | SiegeStartMonth |
| 21 | 1 | Byte |  | SiegeStartDay |
| 22 | 1 | Byte |  | SiegeStartHour |
| 23 | 1 | Byte |  | SiegeStartMinute |
| 24 | 8 | String |  | GuildName |
| 32 | 10 | String |  | GuildMasterName |
| 42 | 4 | IntegerBigEndian |  | RemainingTime |