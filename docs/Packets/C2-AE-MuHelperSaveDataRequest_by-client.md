# C2 AE - MuHelperSaveDataRequest (by client)

## Is sent when

The client want to save current MU Helper data.

## Causes the following actions on the server side

The server should save supplied MU Helper data.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |   261   | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0xAE  | Packet header - packet type identifier |
| 4 | 257 | Binary |  | HelperData |