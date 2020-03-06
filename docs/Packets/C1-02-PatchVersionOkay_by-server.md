# C1 02 - PatchVersionOkay (by server)

## Is sent when

This packet is sent by the server after the client (launcher) requested the to check the patch version and it was high enough.

## Causes the following actions on the client side

The launcher will activate its start button.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x02  | Packet header - packet type identifier |