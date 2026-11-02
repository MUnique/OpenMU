# C1 96 - MutoNumbers (by server)

## Is sent when

The server sends the lottery (muto) numbers of the Lena event.

## Causes the following actions on the client side

The client stores the numbers if none of them is -1.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   10   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x96  | Packet header - packet type identifier |
| 4 | 2 | ShortLittleEndian |  | Number1 |
| 6 | 2 | ShortLittleEndian |  | Number2 |
| 8 | 2 | ShortLittleEndian |  | Number3 |