# C2 AE - MuHelperConfigurationData (by server)

## Is sent when

The server saved the users MU Helper data.

## Causes the following actions on the client side

The user wants to save the MU Helper data.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |   261   | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0xAE  | Packet header - packet type identifier |
| 4 | 257 | Binary |  | HelperData |