# C1 40 - PartyInviteRequest (by client)

## Is sent when

A party master wants to invite another player to his party.

## Causes the following actions on the server side

If the requesting player has no party, or is the party master, a request is sent to the target player.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x40  | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | TargetPlayerId |