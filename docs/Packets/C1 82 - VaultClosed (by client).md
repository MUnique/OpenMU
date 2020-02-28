# C1 82 - VaultClosed (by client)

## Is sent when

The player closed an opened vault dialog.

## Causes the following actions on the server side

The state on the server is updated.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x82  | Packet header - packet type identifier |