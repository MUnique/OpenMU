# C1 41 - PartyRequestResult (by server)

## Is sent when

After a party request could not be fulfilled or a party action finished.

## Causes the following actions on the client side

The client shows a message depending on the result (0 to 8).

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x41  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | Result |