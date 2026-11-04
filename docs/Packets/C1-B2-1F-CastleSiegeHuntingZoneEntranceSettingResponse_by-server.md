# C1 B2 1F - CastleSiegeHuntingZoneEntranceSettingResponse (by server)

## Is sent when

The castle owner requests to change public access to the Castle Siege hunting zone.

## Causes the following actions on the client side

The client updates the hunting-zone access setting or shows an error.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x1F  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Result |
| 5 | 1 | Boolean |  | IsPublic |