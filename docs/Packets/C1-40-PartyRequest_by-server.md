# C1 40 - PartyRequest (by server)

## Is sent when

Another player requests party from the receiver of this message.

## Causes the following actions on the client side

The party request is shown.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x40  | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | RequesterId |