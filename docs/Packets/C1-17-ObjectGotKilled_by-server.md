# C1 17 - ObjectGotKilled (by server)

## Is sent when

An observed object was killed.

## Causes the following actions on the client side

The object is shown as dead.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   9   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x17  | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | KilledId |
| 7 | 2 | ShortBigEndian |  | KillerId |