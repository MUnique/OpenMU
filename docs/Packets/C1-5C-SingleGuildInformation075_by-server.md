# C1 5C - SingleGuildInformation075 (by server)

## Is sent when

After a guild has been created. However, in OpenMU, we just send the GuildInformations075 message, because it works just the same.

## Causes the following actions on the client side

The players which belong to the guild are shown as guild players.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   45   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x5C  | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | GuildId |
| 5 | 8 | String |  | GuildName |
| 13 | 32 | Binary |  | Logo |