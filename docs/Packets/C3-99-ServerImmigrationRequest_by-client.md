# C3 99 - ServerImmigrationRequest (by client)

## Is sent when

Unknown?

## Causes the following actions on the server side

Unknown?

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x99  | Packet header - packet type identifier |
| 3 |  | String |  | SecurityCode |