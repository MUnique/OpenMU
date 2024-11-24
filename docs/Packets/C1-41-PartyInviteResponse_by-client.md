# C1 41 - PartyInviteResponse (by client)

## Is sent when

A player was invited by another player to join a party and this player sent the response back.

## Causes the following actions on the server side

If the sender accepts the request, it's added to the party.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x41  | Packet header - packet type identifier |
| 3 | 1 | Boolean |  | Accepted |
| 4 | 2 | ShortBigEndian |  | RequesterId |