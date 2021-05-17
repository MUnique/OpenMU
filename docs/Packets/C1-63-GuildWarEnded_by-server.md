# C1 63 - GuildWarEnded (by server)

## Is sent when

The guild war ended.

## Causes the following actions on the client side

The guild war is shown as ended on the client side.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   12   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x63  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | Result |
| 4 | 8 | String |  | GuildName |