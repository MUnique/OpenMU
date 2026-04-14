# C1 B9 02 - CastleOwnerLogo (by server)

## Is sent when

After the client requested the guild logo of the current castle owner.

## Causes the following actions on the client side

The client shows the castle owner guild logo.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   36   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB9  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x02  | Packet header - sub packet type identifier |
| 4 | 32 | Binary |  | Logo |