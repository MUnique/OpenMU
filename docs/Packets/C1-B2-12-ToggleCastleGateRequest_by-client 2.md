# C1 B2 12 - ToggleCastleGateRequest (by client)

## Is sent when

The guild member of the castle owner wants to toggle the gate switch.

## Causes the following actions on the server side

The castle gate is getting opened or closed.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   7   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x12  | Packet header - sub packet type identifier |
| 4 | 1 | Boolean |  | CloseState |
| 5 | 2 | ShortBigEndian |  | GateId |