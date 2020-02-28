# C1 05 1 - ClientNeedsPatch (by server)

## Is sent when

This packet is sent by the server after the client (launcher) requested to check the patch version and it requires an update.

## Causes the following actions on the client side

The launcher will download the required patches and then activate the start button.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   138   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x05  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x1  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | PatchVersion |
| 6 |  | String |  | PatchAddress; The patch address, usually to a ftp server. The address is usually "encrypted" with the 3-byte XOR key (FC CF AB). |